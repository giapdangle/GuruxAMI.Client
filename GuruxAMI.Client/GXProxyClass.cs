//
// --------------------------------------------------------------------------
//  Gurux Ltd
// 
//
//
// Filename:        $HeadURL$
//
// Version:         $Revision$,
//                  $Date$
//                  $Author$
//
// Copyright (c) Gurux Ltd
//
//---------------------------------------------------------------------------
//
//  DESCRIPTION
//
// This file is a part of Gurux Device Framework.
//
// Gurux Device Framework is Open Source software; you can redistribute it
// and/or modify it under the terms of the GNU General Public License 
// as published by the Free Software Foundation; version 2 of the License.
// Gurux Device Framework is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
// See the GNU General Public License for more details.
//
// This code is licensed under the GNU General Public License v2. 
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GuruxAMI.Client;
using GuruxAMI.Common;
using System.Threading;
using System.IO;
using System.Reflection;
using Gurux.Device;
using Gurux.Device.Editor;
using Gurux.Common;
using GuruxAMI.Common.Messages;
using System.ComponentModel;

namespace GuruxAMI.Client
{
    /// <summary>
    /// Load Device Template to own namespace.
    /// </summary>
    class GXProxyClass : MarshalByRefObject
    {
        GXAmiEventListener.UpdateparametersEventHandler m_UpdateParameters;
        GXAmiEventListener.PropertyUpdateEventHandler m_Updated;
        GXAmiEventListener.ErrorEventHandler m_Error;
        GXAmiEventListener.DeviceStateChangedEventHandler m_StateChange;
        GXAmiEventListener.TraceAddEventHandler m_Trace;

        public event GXAmiEventListener.PropertyUpdateEventHandler OnUpdated
        {
            add
            {
                m_Updated += value;
            }
            remove
            {
                m_Updated -= value;
            }
        }

        public event GXAmiEventListener.UpdateparametersEventHandler OnUpdateParameters
        {
            add
            {
                m_UpdateParameters += value;
            }
            remove
            {
                m_UpdateParameters -= value;
            }
        }

        public event GXAmiEventListener.ErrorEventHandler OnError
        {
            add
            {
                m_Error += value;
            }
            remove
            {
                m_Error -= value;
            }
        }

        public event GXAmiEventListener.DeviceStateChangedEventHandler OnStateChange
        {
            add
            {
                m_StateChange += value;
            }
            remove
            {
                m_StateChange -= value;
            }
        }

        public event GXAmiEventListener.TraceAddEventHandler OnTrace
        {
            add
            {
                m_Trace += value;
            }
            remove
            {
                m_Trace -= value;
            }
        }        

        /// <summary>
        /// Save executed task. This is used if error occures.
        /// </summary>        
        GXClaimedTask ExecutedTask;
        GXDeviceList DeviceList;
        GXDevice Device;
        string TargetDirectory;
        System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            foreach (string it in Directory.GetFiles(TargetDirectory, "*.dll"))
            {
                Assembly asm = Assembly.LoadFile(it);
                if (asm.GetName().ToString() == args.Name)
                {
                    return asm;
                }
            }
            return null;
        }

        static Assembly CurrentDomain_TypeResolve(object sender, ResolveEventArgs args)
        {
            int pos = args.Name.LastIndexOf('.');
            if (pos != -1)
            {
                string ns = args.Name.Substring(0, pos);
                string className = args.Name.Substring(pos + 1);
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.GetName().Name == ns)
                    {
                        if (assembly.GetType(args.Name, false, true) != null)
                        {
                            return assembly;
                        }
                    }
                }
            }
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.Name == args.Name ||
                        type.FullName == args.Name)
                    {
                        return assembly;
                    }
                }
            }
            System.Diagnostics.Debug.WriteLine(args.Name);
            return null;
        }

        public GXDeviceProfile Import(byte[] data, string path)
        {
            return GXZip.Import(null, data, path + "\\");
        }

        bool ShouldSerialize(object value1, object value2)
        {
            if (value1 == null || value2 == null)
            {
                return value1 != value2;
            }
            return !value1.Equals(value2);
        }

        void UpdateValues(GXAmiParameter[] parameters, object target, List<GXAmiParameter> list)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(target);
            object value;
            foreach (GXAmiParameter dp in parameters)
            {
                value = properties[dp.Name].GetValue(target);
                if (ShouldSerialize(value, dp.Value))
                {
                    dp.Value = value;
                    list.Add(dp);
                }
            }
        }

        /// <summary>
        /// Disconnect from the media.
        /// </summary>            
        public void Disconnect()
        {
            //If already connected.
            if (Device == null)
            {
                return;
            }
            //Only changed parameters are updated.
            //Note! Parameters are updated ONLY when whole device is read.
            //If parameters are updated when part of device is read. Example table total data collection might damage.
            if (ExecutedTask.Task.TargetType == TargetType.Device && m_UpdateParameters != null)
            {
                List<GXAmiParameter> parameters = new List<GXAmiParameter>();
                //Save parameters.
                UpdateValues(ExecutedTask.Device.Parameters, Device, parameters);               
                foreach (GXAmiCategory cat in ExecutedTask.Device.Categories)
                {                    
                    GXCategory tmpCat = Device.Categories[cat.Name];
                    UpdateValues(cat.Parameters, tmpCat, parameters);
                    foreach (GXAmiProperty prop in cat.Properties)
                    {
                        GXProperty tmpProp = tmpCat.Properties.Find(prop.Name, null);
                        UpdateValues(prop.Parameters, tmpProp, parameters);
                    }
                }
                foreach (GXAmiDataTable table in ExecutedTask.Device.Tables)
                {
                    GXTable tmpTable = Device.Tables[table.Name];
                    UpdateValues(table.Parameters, tmpTable, parameters);
                    foreach (GXAmiProperty prop in table.Columns)
                    {
                        GXProperty tmpProp = tmpTable.Columns.Find(prop.Name, null);
                        UpdateValues(prop.Parameters, tmpProp, parameters);
                    }
                }
                m_UpdateParameters(parameters.ToArray());
            }
            Device.Disconnect();
        }

        public void Close()
        {
            Disconnect();
            if (DeviceList != null)
            {
                DeviceList.Dispose();
                DeviceList = null;
            }
        }

        void UpdateParameter(GXAmiParameter[] parameters, object target)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(target);
            foreach (GXAmiParameter it in parameters)
            {
                if (it.Type != null && it.Value != null)
                {
                    if (it.Type.IsEnum)
                    {
                        it.Value = Enum.Parse(it.Type, it.Value.ToString());
                    }
                    else if (it.Type.GetType() != typeof(object))
                    {
                        it.Value = Convert.ChangeType(it.Value, it.Type);
                    }
                }
                properties[it.Name].SetValue(target, it.Value);
            }
        }

        void UpdateParameters()
        {
            UpdateParameter(ExecutedTask.Device.Parameters, Device);           
            foreach (GXAmiCategory cat in ExecutedTask.Device.Categories)
            {
                GXCategory tmpCat = Device.Categories[cat.Name];
                UpdateParameter(cat.Parameters, tmpCat);
                foreach (GXAmiProperty prop in cat.Properties)
                {
                    GXProperty tmpProp = tmpCat.Properties.Find(prop.Name, null);
                    UpdateParameter(prop.Parameters, tmpProp);
                }                      
            }
            foreach (GXAmiDataTable table in ExecutedTask.Device.Tables)
            {
                GXTable tmpTable = Device.Tables[table.Name];
                UpdateParameter(table.Parameters, tmpTable);
                foreach (GXAmiProperty prop in table.Columns)
                {
                    GXProperty tmpProp = tmpTable.Columns.Find(prop.Name, null);
                    UpdateParameter(prop.Parameters, tmpProp);
                }
            }
        }

        /// <summary>
        /// Load device template and connect to the device.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="taskinfo"></param>
        public void Connect(string path, GXClaimedTask taskinfo)
        {
            ExecutedTask = taskinfo;
            //If already connected.
            if (Device != null)
            {
                UpdateParameters();
                if ((Device.Status & DeviceStates.Connected) == 0)
                {
                    Device.Connect();
                }
                return;
            }
            DeviceList = new GXDeviceList();
            DeviceList.OnError += new Gurux.Common.ErrorEventHandler(DeviceList_OnError);
            DeviceList.OnUpdated += new ItemUpdatedEventHandler(DeviceList_OnUpdated);                        
            GXDeviceGroup group = new GXDeviceGroup();
            DeviceList.DeviceGroups.Add(group);
            TargetDirectory = path;
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            AppDomain.CurrentDomain.TypeResolve += new ResolveEventHandler(CurrentDomain_TypeResolve);
            GXDeviceList.Update(path);
            string filename = Path.Combine(path, taskinfo.Device.ProfileGuid + ".gxp");            
            Device = GXDevice.Load(filename);
            Device.ID = taskinfo.Device.Id;
            group.Devices.Add(Device);
            Device.Name = taskinfo.Device.Name;
            //taskinfo.Device.AutoConnect;
            // ForcePerPropertyRead
            Device.UpdateInterval = taskinfo.Device.UpdateInterval;
            Device.WaitTime = taskinfo.Device.WaitTime;
            Device.ResendCount = taskinfo.Device.ResendCount;
            Device.DisabledActions = taskinfo.Device.DisabledActions;

            /* TODO:
            Device.FailTryCount = taskinfo.Device.FailTryCount;
            Device.FailWaitTime = taskinfo.Device.FailWaitTime;
            Device.ConnectionTryCount = taskinfo.Device.ConnectionTryCount;
            Device.ConnectionFailWaitTime = taskinfo.Device.ConnectionFailWaitTime;
              */

            //Update parameters.
            UpdateParameters();

            //Load medias to this assembly domin.
            Gurux.Communication.GXClient.GetAvailableMedias();
            if (taskinfo.Device.TraceLevel != System.Diagnostics.TraceLevel.Off)
            {
                Device.Trace = taskinfo.Device.TraceLevel;
                Device.OnTrace += new TraceEventHandler(Device_OnTrace);
            }
            Exception lastException = null;
            int pos = -1;
            Gurux.Common.IGXMedia media = null;
            foreach (var it in taskinfo.MediaSettings)
            {
                try
                {
                    ++pos;
                    //If media is changed.
                    if (media == null || media.MediaType != taskinfo.MediaSettings[pos].Key)
                    {
                        media = Device.GXClient.SelectMedia(taskinfo.MediaSettings[pos].Key);
                        Device.GXClient.AssignMedia(media);
                    }
                    media.Settings = taskinfo.MediaSettings[pos].Value;                    
                    lastException = null;                        
                    Device.Connect();
                    break;
                }
                catch (Exception ex)
                {
                    //If connection fails try next redundant connectio.
                    lastException = ex;                    
                }
            }
            if (lastException != null)
            {
                throw lastException;
            }
        }

        /// <summary>
        /// Send device trace to the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Device_OnTrace(object sender, TraceEventArgs e)
        {
            if (m_Trace != null)
            {
                GXAmiTrace trace = new GXAmiTrace();
                trace.DeviceId = Device.ID;
                trace.Type = e.Type;
                if (e.Data is byte[])
                {
                    trace.Data = BitConverter.ToString(e.Data as byte[]).Replace('-', ' ');
                }
                else if (e.Data != null)
                {
                    trace.Data = Convert.ToString(e.Data);
                }
                if (trace.Data != null)
                {
                    trace.DataType = trace.Data.GetType().FullName;
                }
                m_Trace(new GXAmiTrace[] { trace });
            }
        }

        /// <summary>
        /// Update readed values to the DB.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DeviceList_OnUpdated(object sender, GXItemEventArgs e)
        {
            if (e.Item is GXProperty)
            {
                GXPropertyEventArgs t = e as GXPropertyEventArgs;
                //Notify if property read or write is started or ended.
                if ((t.Status & PropertyStates.ValueChanged) == 0)
                {
                    return;
                }
                m_Updated(new GXAmiDataValue[] { new GXAmiDataValue(t.Item.ID, Convert.ToString(t.Item.GetValue(true)), t.Item.ReadTime) });
            }
            else if (e.Item is GXTable)
            {
                GXTableEventArgs t = e as GXTableEventArgs;
                //Notify if property read or write is started or ended.
                if ((t.Status & (TableStates.RowsAdded | TableStates.RowsUpdated)) == 0)
                {
                    return;
                }
                List<GXAmiDataRow> values = new List<GXAmiDataRow>();
                foreach (object[] row in t.Rows)
                {
                    int col = -1;
                    foreach (object value in row)
                    {
                        ++col;
                        values.Add(new GXAmiDataRow(t.Item.ID, t.Item.Columns[col].ID, Convert.ToString(value), 0, (uint)col));
                    }
                }
                m_Updated(values.ToArray());
            }
            else if (e.Item is GXDevice)
            {
                GXDeviceEventArgs t = e as GXDeviceEventArgs;
                if ((t.Status & (DeviceStates.Disconnected | DeviceStates.Connected)) == 0)
                {
                    return;
                }
                //Set only monitoring flag. There is no need to show all flags.
                if ((t.Status & (DeviceStates.Monitoring)) != 0)
                {
                    m_StateChange(ExecutedTask.Device.Id, DeviceStates.Monitoring);
                }
                else
                {
                    m_StateChange(ExecutedTask.Device.Id, t.Status);
                }
            }
        }

        void DeviceList_OnError(object sender, Exception ex)
        {
            m_Error(ExecutedTask.Task, ex);
        }

        public void StartMonitoring()
        {
            Device.StartMonitoring();
        }

        public void StopMonitoring()
        {
            Device.StopMonitoring();
        }

        public void ExecuteTransaction(GXClaimedTask taskinfo)
        {            
            object target;
            if (taskinfo.Task.TargetType == TargetType.Device)
            {
                target = Device;
            }
            else
            {                
                target = Device.FindItemByID(taskinfo.Task.TargetID);
            }
            if (target is GXDevice)
            {
                if (taskinfo.Task.TaskType == TaskType.Read)
                {
                    (target as GXDevice).Read();
                }
            }
            else if (target is GXCategory)
            {
                if (taskinfo.Task.TaskType == TaskType.Read)
                {
                    (target as GXCategory).Read();
                }
            }
            else if (target is GXTable)
            {
                if (!string.IsNullOrEmpty(taskinfo.Task.Data))
                {
                    Gurux.Device.Editor.IGXPartialRead pr = target as Gurux.Device.Editor.IGXPartialRead;
                    string[] parameters = taskinfo.Task.Data.Split(new char[] { ';' });                    
                    pr.Type = (PartialReadType) Convert.ToInt32(parameters[0]);
                    //Read by entry (index and count)..
                    if (pr.Type == PartialReadType.Entry)
                    { 
                        int start = Convert.ToInt32(parameters[1]);
                        int end = Convert.ToInt32(parameters[2]);
                        //Read all values.
                        if (start == 0 && end == 0)
                        {
                            pr.Type = PartialReadType.All;
                        }
                        else
                        {
                            pr.Start = start;
                            pr.End = end;
                        }
                    }
                    //Read by range (between start and end time).
                    else if (pr.Type == PartialReadType.Range)
                    {
                        pr.Start = Convert.ToDateTime(parameters[1]);
                        pr.End = Convert.ToDateTime(parameters[2]);
                    }
                    else
                    {
                        throw new ArgumentException("Invalid type.");
                    }
                }
                //In default read new values.
                if (taskinfo.Task.TaskType == TaskType.Read)
                {
                    (target as GXTable).Read();
                }
            }
            else if (target is GXProperty)
            {
                if (taskinfo.Task.TaskType == TaskType.Read)
                {
                    (target as GXProperty).Read();
                }
            }
            else
            {
                throw new ArgumentException("Unknown target.");
            }
        }
    }
}
