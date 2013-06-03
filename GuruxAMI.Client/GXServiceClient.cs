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
using ServiceStack.ServiceClient.Web;
using GuruxAMI.Common;
using GuruxAMI.Common.Messages;
using Gurux.Device.Editor;
using System.IO;
using System.Net.NetworkInformation;
using Gurux.Device;
using System.Threading;
using System.Reflection;
using Gurux.Common;

namespace GuruxAMI.Client
{
    /// <summary>
    /// New user is added.
    /// </summary>
    /// <param name="sender"></param>
    public delegate void UsersAddedEventHandler(object sender, GXAmiUser[] users);

    /// <summary>
    /// User is updated.
    /// </summary>
    /// <param name="sender"></param>
    public delegate void UsersUpdatedEventHandler(object sender, GXAmiUser[] users);

    /// <summary>
    /// User is removed.
    /// </summary>
    /// <param name="sender"></param>
    public delegate void UsersRemovedEventHandler(object sender, GXAmiUser[] users);

    /// <summary>
    /// New user group(s) are added.
    /// </summary>
    /// <param name="sender"></param>
    public delegate void UserGroupsAddedEventHandler(object sender, GXAmiUserGroup[] groups);

    /// <summary>
    /// User group(s) are updated.
    /// </summary>
    /// <param name="sender"></param>
    public delegate void UserGroupsUpdatedEventHandler(object sender, GXAmiUserGroup[] groups);

    /// <summary>
    /// User group(s) are removed.
    /// </summary>
    /// <param name="sender"></param>
    public delegate void UserGroupsRemovedEventHandler(object sender, GXAmiUserGroup[] groups);
    
    /// <summary>
    /// New device template(s) are added.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="devices">Added devices.</param>    
    public delegate void DeviceTemplatesAddedEventHandler(object sender, GXAmiDeviceTemplate[] templates);

    /// <summary>
    /// GXAmiDevice template(s) are removed.
    /// </summary>
    /// <param name="sender"></param>
    public delegate void DeviceTemplatesRemovedEventHandler(object sender, GXAmiDeviceTemplate[] templates);

    /// <summary>
    /// New device(s) are added to the device group.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="target">GXAmiDevice Group or data collector where devices are added.</param>
    /// <param name="devices">Added devices.</param>    
    public delegate void DevicesAddedEventHandler(object sender, object target, GXAmiDevice[] devices);

    /// <summary>
    /// Device(s) are updated.
    /// </summary>
    /// <param name="sender"></param>
    public delegate void DevicesUpdatedEventHandler(object sender, GXAmiDevice[] devices);

    /// <summary>
    /// Device(s) are removed.
    /// </summary>
    /// <param name="sender"></param>
    public delegate void DevicesRemovedEventHandler(object sender, GXAmiDevice[] devices);

    /// <summary>
    /// New device group(s) are added.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="deviceGrops">Added device groups.</param>
    /// <param name="group">Group where items are added.</param>
    public delegate void DeviceGroupsAddedEventHandler(object sender, GXAmiDeviceGroup[] deviceGrops, GXAmiDeviceGroup group);

    /// <summary>
    /// GXAmiDevice group(s) are updated.
    /// </summary>
    /// <param name="sender"></param>
    public delegate void DeviceGroupsUpdatedEventHandler(object sender, GXAmiDeviceGroup[] deviceGrops);

    /// <summary>
    /// GXAmiDevice group(s) are removed.
    /// </summary>
    /// <param name="sender"></param>
    public delegate void DeviceGroupsRemovedEventHandler(object sender, GXAmiDeviceGroup[] deviceGrops);

    /// <summary>
    /// New task(s) are added.
    /// </summary>
    /// <param name="sender"></param>
    public delegate void TasksAddedEventHandler(object sender, GXAmiTask[] tasks);

    /// <summary>
    /// Task(s) are claimed.
    /// </summary>
    /// <param name="sender"></param>
    public delegate void TasksClaimedEventHandler(object sender, GXAmiTask[] tasks);

    /// <summary>
    /// Task(s) are removed.
    /// </summary>
    /// <param name="sender"></param>
    public delegate void TasksRemovedEventHandler(object sender, GXAmiTask[] tasks);

    /// <summary>
    /// GXAmiDevice error(s) are added.
    /// </summary>
    /// <param name="sender"></param>
    public delegate void DeviceErrorsAddedEventHandler(object sender, GXAmiDeviceError[] errors);

    /// <summary>
    /// GXAmiDevice error(s) are removed.
    /// </summary>
    /// <param name="sender"></param>
    public delegate void DeviceErrorsRemovedEventHandler(object sender, GXAmiDeviceError[] errors);

    /// <summary>
    /// New System error(s) are occurred.
    /// </summary>
    /// <param name="sender"></param>
    public delegate void SystemErrorsAddedEventHandler(object sender, GXAmiSystemError[] errors);

    /// <summary>
    /// System error(s) are removed.
    /// </summary>
    /// <param name="sender"></param>
    public delegate void SystemErrorsRemovedEventHandler(object sender, GXAmiSystemError[] errors);

    /// <summary>
    /// Value of the property is updated.
    /// </summary>
    /// <param name="sender"></param>
    public delegate void ValueUpdatedEventHandler(object sender, GXAmiDataValue[] values);

    /// <summary>
    /// Table values are changed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="TableId">Table ID.</param>
    /// <param name="rowIndex">Index where rows are added. If rows are updated those rows can be replaced.</param>
    /// <param name="values">rows</param>
    public delegate void TableValuesUpdatedEventHandler(object sender, ulong TableId, uint rowIndex, object[][] values);

    /// <summary>
    /// New data collector(s) are added.
    /// </summary>
    /// <param name="sender"></param>
    public delegate void DataCollectorsAddedEventHandler(object sender, GXAmiDataCollector[] collectors);

    /// <summary>
    /// Data collectors(s) are updated.
    /// </summary>
    /// <param name="sender"></param>
    public delegate void DataCollectorsUpdatedEventHandler(object sender, GXAmiDataCollector[] collectors);

    /// <summary>
    /// Data collector(s) are removed.
    /// </summary>
    /// <param name="sender"></param>
    public delegate void DataCollectorsRemovedEventHandler(object sender, GXAmiDataCollector[] collectors);

    /// <summary>
    /// Data collector(s) states are changed.
    /// </summary>
    /// <param name="sender"></param>
    public delegate void DataCollectorsStateChangedEventHandler(object sender, GXAmiDataCollector[] collectors);

    /// <summary>
    /// Device(s) states are changed.
    /// </summary>
    /// <param name="sender"></param>
    public delegate void DeviceStateChangedEventHandler(object sender, GXAmiDevice[] devices);

    /// <summary>
    /// Device or Data Collector is added new trace(s).
    /// </summary>
    /// <param name="sender"></param>
    public delegate void TraceAddedEventHandler(object sender, GXAmiTrace[] e);

    /// <summary>
    /// Device or Data Collector trace state is changed.
    /// </summary>
    /// <param name="sender"></param>
    public delegate void TraceStateChangedEventHandler(object sender, Guid dataCollector, ulong deviceId, System.Diagnostics.TraceLevel level);

    /// <summary>
    /// Traces(s) are cleared.
    /// </summary>
    /// <param name="sender"></param>
    public delegate void TraceClearEventHandler(object sender, Guid dataCollector, ulong deviceId);

    public class GXAmiClient : IDisposable
    {
        public string Name;
		/// <summary>
		/// There can be only one listener per process.
		/// </summary>
        static Guid ListenerGuid;
        static List<GXAmiClient> Listeners;
        JsonServiceClient Client;
        //Events
        DeviceTemplatesAddedEventHandler m_DeviceTemplatesAdded;
        DeviceTemplatesRemovedEventHandler m_DeviceTemplatesRemoved;
        UsersAddedEventHandler m_UserAdded;
        UsersUpdatedEventHandler m_UsersUpdated;
        UsersRemovedEventHandler m_UsersRemoved;
        UserGroupsAddedEventHandler m_UserGroupsAdded;
        UserGroupsUpdatedEventHandler m_UserGroupsUpdated;
        UserGroupsRemovedEventHandler m_UserGroupsRemoved;
        DevicesAddedEventHandler m_DevicesAdded;
        DevicesUpdatedEventHandler m_DevicesUpdated;
        DevicesRemovedEventHandler m_DevicesRemoved;
        DeviceGroupsAddedEventHandler m_DeviceGroupsAdded;
        DeviceGroupsUpdatedEventHandler m_DeviceGroupsUpdated;
        DeviceGroupsRemovedEventHandler m_DeviceGroupsRemoved;
        TasksAddedEventHandler m_TasksAdded;
        TasksClaimedEventHandler m_TasksClaimed;
        TasksRemovedEventHandler m_TasksRemoved;
        DeviceErrorsAddedEventHandler m_DeviceErrorsAdded;
        DeviceErrorsRemovedEventHandler m_DeviceErrorsRemoved;
        SystemErrorsAddedEventHandler m_SystemErrorsAdded;
        SystemErrorsRemovedEventHandler m_SystemErrorsRemoved;
        ValueUpdatedEventHandler m_ValueUpdated;
        TableValuesUpdatedEventHandler m_TableValuesUpdated;
        DataCollectorsAddedEventHandler m_DataCollectorsAdded;
        DataCollectorsUpdatedEventHandler m_DataCollectorsUpdated;
        DataCollectorsRemovedEventHandler m_DataCollectorsRemoved;
        DataCollectorsStateChangedEventHandler m_DataCollectorStateChanged;
        DeviceStateChangedEventHandler m_DeviceStateChanged;
        TraceAddedEventHandler m_TraceAdded;
        TraceStateChangedEventHandler m_TraceStateChanged;
        TraceClearEventHandler m_TraceClear;

        /// <summary>
        /// New user is added.
        /// </summary>
        public event UsersAddedEventHandler OnUsersAdded
        {
            add
            {
                m_UserAdded += value;
            }
            remove
            {
                m_UserAdded -= value;
            }
        }

        /// <summary>
        /// User is updated.
        /// </summary>
        /// <param name="sender"></param>
        public event UsersUpdatedEventHandler OnUsersUpdated
        {
            add
            {
                m_UsersUpdated += value;
            }
            remove
            {
                m_UsersUpdated -= value;
            }
        }

        /// <summary>
        /// User is removed.
        /// </summary>
        /// <param name="sender"></param>
        public event UsersRemovedEventHandler OnUsersRemoved
        {
            add
            {
                m_UsersRemoved += value;

            }
            remove
            {
                m_UsersRemoved -= value;
            }
        }


        /// <summary>
        /// New user group(s) are added.
        /// </summary>
        /// <param name="sender"></param>
        public event UserGroupsAddedEventHandler OnUserGroupsAdded
        {
            add
            {
                m_UserGroupsAdded += value;
            }
            remove
            {
                m_UserGroupsAdded -= value;
            }
        }

        /// <summary>
        /// User group(s) are updated.
        /// </summary>
        /// <param name="sender"></param>
        public event UserGroupsUpdatedEventHandler OnUserGroupsUpdated
        {
            add
            {
                m_UserGroupsUpdated += value;
            }
            remove
            {
                m_UserGroupsUpdated -= value;
            }
        }

        /// <summary>
        /// User group(s) are removed.
        /// </summary>
        /// <param name="sender"></param>
        public event UserGroupsRemovedEventHandler OnUserGroupsRemoved
        {
            add
            {
                m_UserGroupsRemoved += value;
            }
            remove
            {
                m_UserGroupsRemoved -= value;
            }
        }

        /// <summary>
        /// New device templates(s) are added.
        /// </summary>
        /// <param name="sender"></param>
        public event DeviceTemplatesAddedEventHandler OnDeviceTemplatesAdded
        {
            add
            {
                m_DeviceTemplatesAdded += value;
            }
            remove
            {
                m_DeviceTemplatesAdded -= value;
            }
        }

        /// <summary>
        /// GXAmiDevice templates (s) are removed.
        /// </summary>
        /// <param name="sender"></param>
        public event DeviceTemplatesRemovedEventHandler OnDeviceTemplatesRemoved
        {
            add
            {
                m_DeviceTemplatesRemoved += value;
            }
            remove
            {
                m_DeviceTemplatesRemoved -= value;
            }
        }


        /// <summary>
        /// New device(s) are added.
        /// </summary>
        /// <param name="sender"></param>
        public event DevicesAddedEventHandler OnDevicesAdded
        {
            add
            {
                m_DevicesAdded += value;
            }
            remove
            {
                m_DevicesAdded -= value;
            }
        }

        /// <summary>
        /// Device(s) are updated.
        /// </summary>
        /// <param name="sender"></param>
        public event DevicesUpdatedEventHandler OnDevicesUpdated
        {
            add
            {
                m_DevicesUpdated += value;
            }
            remove
            {
                m_DevicesUpdated -= value;
            }
        }

        /// <summary>
        /// Device(s) are removed.
        /// </summary>
        /// <param name="sender"></param>
        public event DevicesRemovedEventHandler OnDevicesRemoved
        {
            add
            {
                m_DevicesRemoved += value;
            }
            remove
            {
                m_DevicesRemoved -= value;
            }
        }
        /// <summary>
        /// New device group(s) are added.
        /// </summary>
        /// <param name="sender"></param>
        public event DeviceGroupsAddedEventHandler OnDeviceGroupsAdded
        {
            add
            {
                m_DeviceGroupsAdded += value;
            }
            remove
            {
                m_DeviceGroupsAdded -= value;
            }
        }
        /// <summary>
        /// GXAmiDevice group(s) are updated.
        /// </summary>
        /// <param name="sender"></param>
        public event DeviceGroupsUpdatedEventHandler OnDeviceGroupsUpdated
        {
            add
            {
                m_DeviceGroupsUpdated += value;
            }
            remove
            {
                m_DeviceGroupsUpdated -= value;
            }
        }
        /// <summary>
        /// GXAmiDevice group(s) are removed.
        /// </summary>
        /// <param name="sender"></param>
        public event DeviceGroupsRemovedEventHandler OnDeviceGroupsRemoved
        {
            add
            {
                m_DeviceGroupsRemoved += value;
            }
            remove
            {
                m_DeviceGroupsRemoved -= value;
            }
        }
        /// <summary>
        /// New task(s) are added.
        /// </summary>
        /// <param name="sender"></param>
        public event TasksAddedEventHandler OnTasksAdded
        {
            add
            {
                m_TasksAdded += value;
            }
            remove
            {
                m_TasksAdded -= value;
            }
        }

        /// <summary>
        /// Task(s) are claimed.
        /// </summary>
        /// <param name="sender"></param>
        public event TasksClaimedEventHandler OnTasksClaimed
        {
            add
            {
                m_TasksClaimed += value;
            }
            remove
            {
                m_TasksClaimed -= value;
            }
        }         

        /// <summary>
        /// Task(s) are removed.
        /// </summary>
        /// <param name="sender"></param>
        public event TasksRemovedEventHandler OnTasksRemoved
        {
            add
            {
                m_TasksRemoved += value;
            }
            remove
            {
                m_TasksRemoved -= value;
            }
        }

        /// <summary>
        /// GXAmiDevice error(s) are added.
        /// </summary>
        /// <param name="sender"></param>
        public event DeviceErrorsAddedEventHandler OnDeviceErrorsAdded
        {
            add
            {
                m_DeviceErrorsAdded += value;
            }
            remove
            {
                m_DeviceErrorsAdded -= value;
            }
        }

        /// <summary>
        /// GXAmiDevice error(s) are removed.
        /// </summary>
        /// <param name="sender"></param>
        public event DeviceErrorsRemovedEventHandler OnDeviceErrorsRemoved
        {
            add
            {
                m_DeviceErrorsRemoved += value;
            }
            remove
            {
                m_DeviceErrorsRemoved -= value;
            }
        }

        /// <summary>
        /// New System error(s) are occurred.
        /// </summary>
        /// <param name="sender"></param>
        public event SystemErrorsAddedEventHandler OnSystemErrorsAdded
        {
            add
            {
                m_SystemErrorsAdded += value;
            }
            remove
            {
                m_SystemErrorsAdded -= value;
            }
        }

        /// <summary>
        /// System error(s) are removed.
        /// </summary>
        /// <param name="sender"></param>
        public event SystemErrorsRemovedEventHandler OnSystemErrorsRemoved
        {
            add
            {
                m_SystemErrorsRemoved += value;
            }
            remove
            {
                m_SystemErrorsRemoved -= value;
            }
        }

        /// <summary>
        /// Value of the property is updated.
        /// </summary>
        /// <param name="sender"></param>
        public event ValueUpdatedEventHandler OnValueUpdated
        {
            add
            {
                m_ValueUpdated += value;
            }
            remove
            {
                m_ValueUpdated -= value;
            }
        }

        /// <summary>
        /// Table values are changed.
        /// </summary>
        /// <param name="sender"></param>
        public event TableValuesUpdatedEventHandler OnTableValuesUpdated
        {
            add
            {
                m_TableValuesUpdated += value;
            }
            remove
            {
                m_TableValuesUpdated -= value;
            }
        }        

        /// <summary>
        /// New data collector(s) are added.
        /// </summary>
        /// <param name="sender"></param>
        public event DataCollectorsAddedEventHandler OnDataCollectorsAdded
        {
            add
            {
                m_DataCollectorsAdded += value;
            }
            remove
            {
                m_DataCollectorsAdded -= value;
            }
        }

        /// <summary>
        /// Data collectors(s) are updated.
        /// </summary>
        /// <param name="sender"></param>
        public event DataCollectorsUpdatedEventHandler OnDataCollectorsUpdated
        {
            add
            {
                m_DataCollectorsUpdated += value;
            }
            remove
            {
                m_DataCollectorsUpdated -= value;
            }
        }

        /// <summary>
        /// Data collector(s) are removed.
        /// </summary>
        /// <param name="sender"></param>
        public event DataCollectorsRemovedEventHandler OnDataCollectorsRemoved
        {
            add
            {
                m_DataCollectorsRemoved += value;
            }
            remove
            {
                m_DataCollectorsRemoved -= value;
            }
        }

        /// <summary>
        /// Data collector(s) state are changed.
        /// </summary>
        /// <param name="sender"></param>
        public event DataCollectorsStateChangedEventHandler OnDataCollectorStateChanged
        {
            add
            {
                m_DataCollectorStateChanged += value;
            }
            remove
            {
                m_DataCollectorStateChanged -= value;
            }
        }

        /// <summary>
        /// Device(s) state are changed.
        /// </summary>
        /// <param name="sender"></param>
        public event DeviceStateChangedEventHandler OnDeviceStateChanged
        {
            add
            {
                m_DeviceStateChanged += value;
            }
            remove
            {
                m_DeviceStateChanged -= value;
            }
        }

        /// <summary>
        /// De
        /// </summary>
        public event TraceAddedEventHandler OnTraceAdded
        {
            add
            {
                m_TraceAdded += value;
            }
            remove
            {
                m_TraceAdded -= value;
            }
        }

        public event TraceStateChangedEventHandler OnTraceStateChanged
        {
            add
            {
                m_TraceStateChanged += value;
            }
            remove
            {
                m_TraceStateChanged -= value;
            }
        }

        public event TraceClearEventHandler OnTraceClear
        {
            add
            {
                m_TraceClear += value;
            }
            remove
            {
                m_TraceClear -= value;
            }
        }

        private void Listen()
        {
            GXEventsRequest e1 = new GXEventsRequest(ListenerGuid);
            Client.GetAsync<GXEventsResponse>(e1,
                r =>
                {
                    NotifyEvents(this, r.Actions, r.DataCollectorGuid, e1);
                }, FailOnAsyncError);
        }

        private void FailOnAsyncError<GXEventsResponse>(GXEventsResponse response, Exception ex)
        {            
            //Client closes the connection.
            if (ex is System.Net.WebException && (ex as System.Net.WebException).Status == System.Net.WebExceptionStatus.RequestCanceled)
            {
                return;
            }
         
            //If server is restarted. Register again.
            if (ex is ArgumentNullException)
            {
                if (Listeners != null)
                {
                    lock (Listeners)
                    {
                        Listeners.Remove(this);
                    }
                }
                StartListenEvents();
                return;
            }            
            try
            {
                Listen();                
            }
            //This fails sometimes. Just send again.
            catch (System.NullReferenceException)
            {
                if (m_SystemErrorsAdded != null)
                {
                    GXAmiSystemError e = new GXAmiSystemError();
                    e.ExceptionType = ex.GetType().ToString();
                    e.Message = "Reconnect to the server.";
                    e.TimeStamp = DateTime.Now;
                    m_SystemErrorsAdded(this, new GXAmiSystemError[] { e });
                }   
            }
        }

        public void StopListenEvents()
        {
            if (Listeners != null)
            {
                lock (Listeners)
                {
                    Listeners.Remove(this);
                }
                try
                {
                    GXEventsUnregisterRequest req = new GXEventsUnregisterRequest(this.DataCollectorGuid, GXAmiClient.ListenerGuid);
                    Client.Put(req);
                }
                catch (Exception ex)
                {
                    //Skip exceptions
                    System.Diagnostics.Debug.WriteLine(ex.Message);                    
                }
            }
        }

        /// <summary>
        /// Start listen GuruxAMI events.
        /// </summary>
        /// <remarks>
        /// You must bind events before you can call this.
        /// </remarks>
        public void StartListenEvents()        
        {
            int actionTargets = (int)ActionTargets.None;
            int actions = (int)Actions.None;
            if (m_UserAdded != null)
            {
                actions |= (int)Actions.Add;
                actionTargets |= (int)ActionTargets.User;
            }
            if (m_UsersUpdated != null)
            {
                actions |= (int)Actions.Edit;
                actionTargets |= (int)ActionTargets.User;
            }
            if (m_UsersRemoved != null)
            {
                actions |= (int)Actions.Remove;
                actionTargets |= (int)ActionTargets.User;
            }
            if (m_UserGroupsAdded != null)
            {
                actions |= (int)Actions.Add;
                actionTargets |= (int)ActionTargets.UserGroup;
            }
            if (m_UserGroupsUpdated != null)
            {
                actions |= (int)Actions.Edit;
                actionTargets |= (int)ActionTargets.UserGroup;
            }
            if (m_UserGroupsRemoved != null)
            {
                actions |= (int)Actions.Remove;
                actionTargets |= (int)ActionTargets.UserGroup;
            }
            if (m_DeviceTemplatesAdded != null)
            {
                actions |= (int)Actions.Add;
                actionTargets |= (int)ActionTargets.DeviceTemplate;
            }
            if (m_DeviceTemplatesRemoved != null)
            {
                actions |= (int)Actions.Remove;
                actionTargets |= (int)ActionTargets.DeviceTemplate;
            }
            if (m_DevicesAdded != null)
            {
                actions |= (int)Actions.Add;
                actionTargets |= (int)ActionTargets.Device;
            }
            if (m_DevicesUpdated != null)
            {
                actions |= (int)Actions.Edit;
                actionTargets |= (int)ActionTargets.Device;
            }
            if (m_DevicesRemoved != null)
            {
                actions |= (int)Actions.Remove;
                actionTargets |= (int)ActionTargets.Device;
            }
            if (m_DeviceGroupsAdded != null)
            {
                actions |= (int)Actions.Add;
                actionTargets |= (int)ActionTargets.DeviceGroup;
            }
            if (m_DeviceGroupsUpdated != null)
            {
                actions |= (int)Actions.Edit;
                actionTargets |= (int)ActionTargets.DeviceGroup;
            }
            if (m_DeviceGroupsRemoved != null)
            {
                actions |= (int)Actions.Remove;
                actionTargets |= (int)ActionTargets.DeviceGroup;
            }
            if (m_TasksAdded != null)
            {
                actions |= (int)Actions.Add;
                actionTargets |= (int)ActionTargets.Task;
            }
            if (m_TasksRemoved != null)
            {
                actions |= (int)Actions.Remove;
                actionTargets |= (int)ActionTargets.Task;
            }
            if (m_DeviceErrorsAdded != null)
            {
                actions |= (int)Actions.Add;
                actionTargets |= (int)ActionTargets.DeviceError;
            }
            if (m_DeviceErrorsRemoved != null)
            {
                actions |= (int)Actions.Remove;
                actionTargets |= (int)ActionTargets.DeviceError;
            }
            if (m_SystemErrorsAdded != null)
            {
                actions |= (int)Actions.Add;
                actionTargets |= (int)ActionTargets.SystemError;
            }
            if (m_SystemErrorsRemoved != null)
            {
                actions |= (int)Actions.Remove;
                actionTargets |= (int)ActionTargets.SystemError;
            }
            if (m_ValueUpdated != null)
            {
                actions |= (int)Actions.Add;
                actionTargets |= (int)ActionTargets.ValueChanged;
            }
            if (m_TableValuesUpdated != null)
            {
                actions |= (int)Actions.Add;
                actionTargets |= (int)ActionTargets.TableValueChanged;
            }
            if (m_DataCollectorsAdded != null)
            {
                actions |= (int)Actions.Add;
                actionTargets |= (int)ActionTargets.DataCollector;
            }
            if (m_DataCollectorsUpdated != null)
            {
                actions |= (int)Actions.Edit;
                actionTargets |= (int)ActionTargets.DataCollector;
            }
            if (m_DataCollectorsRemoved != null)
            {
                actions |= (int)Actions.Remove;
                actionTargets |= (int)ActionTargets.DataCollector;
            }
            if (m_DataCollectorStateChanged != null)
            {
                actions |= (int)Actions.State;
                actionTargets |= (int)ActionTargets.DataCollector;
            }
            if (m_DeviceStateChanged != null)
            {
                actions |= (int)Actions.State;
                actionTargets |= (int)ActionTargets.Device;
            }
            if (m_TraceAdded != null)
            {
                actions |= (int)Actions.Add;
                actionTargets |= (int)ActionTargets.Trace;
            }
            if (m_TraceStateChanged != null)
            {
                actions |= (int)Actions.Edit;
                actionTargets |= (int)ActionTargets.Trace;
            }
            if (m_TraceClear != null)
            {
                actions |= (int)Actions.Remove;
                actionTargets |= (int)ActionTargets.Trace;
            }

            if (actions != 0 && actionTargets != 0)
            {
                if (Listeners == null)
				{                    
                    Listeners = new List<GXAmiClient>();
                    lock (Listeners)
                    {                        
                        Listeners.Add(this);
                    }
                    //Register to listen events.
                    ListenerGuid = DataCollectorGuid;                    
                    //If listener is not DC.
                    if (DataCollectorGuid == Guid.Empty)
                    {
                        ListenerGuid = Guid.NewGuid();
                    }
                    GXEventsRegisterRequest e = new GXEventsRegisterRequest((ActionTargets)actionTargets, (Actions)actions, DataCollectorGuid, ListenerGuid);
                    Client.Post(e);
                    //Start listen events.
                    GXEventsRequest e1 = new GXEventsRequest(ListenerGuid);
					Client.GetAsync<GXEventsResponse>(e1,
						r =>
						{
                            NotifyEvents(this, r.Actions, r.DataCollectorGuid, e1);
						}, FailOnAsyncError);
				}
				else
				{
                    lock (Listeners)
                    {
                        Listeners.Add(this);
                    }
                    //Register to listen events.                    
                    GXEventsRegisterRequest e = new GXEventsRegisterRequest((ActionTargets)actionTargets, (Actions)actions, DataCollectorGuid, ListenerGuid);
                    Client.Post(e);
				}
            }
        }

        /// <summary>
        /// Change separated property values to the collection of rows.
        /// </summary>
        /// <returns></returns>
        object[][] ItemsToRows(GXAmiDataRow[] items, out ulong tableId, out uint startRow)
        {
            if (items == null || items.Length == 0)
            {
                startRow = 0;
                tableId = 0;
                return new object[0][];
            }
            uint maxcount = 0;
            startRow = items[0].RowIndex;
            tableId = items[0].TableID;
            uint rowIndex = items[0].RowIndex;
            foreach (GXAmiDataRow it in items)
            {
                
                if (it.ColumnIndex > maxcount)
                {
                    maxcount = it.ColumnIndex;
                }
                if (rowIndex != it.RowIndex)
                {
                    break;
                }
            }
            List<object[]> rows = new List<object[]>();
            object[] row = new object[maxcount + 1];
            //Make columns
            foreach (GXAmiDataRow it in items)
            {
                //Add new row when row index is changing.
                if (it.RowIndex != rowIndex)
                {
                    rows.Add(row);
                    row = new object[maxcount + 1];
                    rowIndex = it.RowIndex;
                }
                row[it.ColumnIndex] = it.UIValue;
            }
            rows.Add(row);
            return rows.ToArray();
        }

        /// <summary>
        /// Notifies client that event is occurred.
        /// </summary>
        /// <param name="actions2"></param>
        /// <param name="e"></param>
        void NotifyEvents(GXAmiClient sender, GXEventsItem[] actions2, Guid target, GXEventsRequest e)
        {
            lock (Listeners)
            {
                foreach (GXAmiClient cl in Listeners)
                {
                    if (cl.DataCollectorGuid == target)
                    {
                        List<GXAmiDeviceTemplate> deviceTemplatesAdd = new List<GXAmiDeviceTemplate>();
                        List<GXAmiDeviceTemplate> deviceTemplatesRemove = new List<GXAmiDeviceTemplate>();
                        List<GXAmiUser> usersAdd = new List<GXAmiUser>();
                        List<GXAmiUserGroup> userGroupsAdd = new List<GXAmiUserGroup>();
                        List<GXAmiDevice> devicesAdd = new List<GXAmiDevice>();
                        List<GXAmiDeviceGroup> deviceGroupsAdd = new List<GXAmiDeviceGroup>();
                        List<GXAmiTask> tasksAdd = new List<GXAmiTask>();
                        List<GXAmiDataCollector> dcsAdd = new List<GXAmiDataCollector>();
                        List<GXAmiDeviceError> deviceErrorAdd = new List<GXAmiDeviceError>();
                        List<GXAmiTrace> traceAdd = new List<GXAmiTrace>();

                        List<GXAmiUser> usersEdit = new List<GXAmiUser>();
                        List<GXAmiUserGroup> userGroupsEdit = new List<GXAmiUserGroup>();
                        List<GXAmiDevice> devicesEdit = new List<GXAmiDevice>();
                        List<GXAmiDeviceGroup> deviceGroupsEdit = new List<GXAmiDeviceGroup>();
                        List<GXAmiTask> tasksClaimed = new List<GXAmiTask>();
                        List<GXAmiDataCollector> dcsEdit = new List<GXAmiDataCollector>();
                        List<GXAmiTrace> traceEdit = new List<GXAmiTrace>();

                        List<GXAmiUser> usersRemove = new List<GXAmiUser>();
                        List<GXAmiUserGroup> userGroupsRemove = new List<GXAmiUserGroup>();
                        List<GXAmiDevice> devicesRemove = new List<GXAmiDevice>();
                        List<GXAmiDeviceGroup> deviceGroupsRemove = new List<GXAmiDeviceGroup>();
                        List<GXAmiTask> tasksRemove = new List<GXAmiTask>();
                        List<GXAmiDataCollector> dcsRemove = new List<GXAmiDataCollector>();
                        List<GXAmiDataValue> values = new List<GXAmiDataValue>();
                        List<GXAmiDataRow> tableRows = new List<GXAmiDataRow>();
                        List<GXAmiDeviceError> deviceErrorRemove = new List<GXAmiDeviceError>();
                        List<GXAmiDevice> deviceStates = new List<GXAmiDevice>();
                        List<GXAmiDataCollector> dcStates = new List<GXAmiDataCollector>();
                        List<GXAmiTrace> traceClear = new List<GXAmiTrace>();

                        GXAmiDeviceGroup deviceGroup = null;
                        GXAmiDeviceGroup deviceGroupGroup = null;
                        foreach (GXEventsItem it in actions2)
                        {
                            try
                            {
                                if (it.Data is GXAmiUser)
                                {
                                    GXAmiUser user = it.Data as GXAmiUser;
                                    if (it.Action == Actions.Add)
                                    {
                                        usersAdd.Add(user);
                                    }
                                    else if (it.Action == Actions.Edit)
                                    {
                                        usersEdit.Add(user);
                                    }
                                    else if (it.Action == Actions.Remove)
                                    {
                                        usersRemove.Add(user);
                                    }
                                }
                                else if (it.Data is GXAmiUserGroup)
                                {
                                    GXAmiUserGroup ug = it.Data as GXAmiUserGroup;
                                    if (it.Action == Actions.Add)
                                    {
                                        userGroupsAdd.Add(ug);
                                    }
                                    else if (it.Action == Actions.Edit)
                                    {
                                        userGroupsEdit.Add(ug);
                                    }
                                    else if (it.Action == Actions.Remove)
                                    {
                                        userGroupsRemove.Add(ug);
                                    }
                                }
                                else if (it.Data is GXAmiDeviceTemplate)
                                {
                                    GXAmiDeviceTemplate dt = it.Data as GXAmiDeviceTemplate;
                                    if (it.Action == Actions.Add)
                                    {
                                        deviceTemplatesAdd.Add(dt);
                                    }
                                    else if (it.Action == Actions.Remove)
                                    {
                                        deviceTemplatesRemove.Add(dt);
                                    }
                                }
                                else if (it.Data is GXAmiDevice)
                                {
                                    GXAmiDevice GXAmiDevice = it.Data as GXAmiDevice;
                                    if (it.Action == Actions.Add)
                                    {
                                        devicesAdd.Add(GXAmiDevice);
                                    }
                                    else if (it.Action == Actions.Edit)
                                    {
                                        devicesEdit.Add(GXAmiDevice);
                                    }
                                    else if (it.Action == Actions.Remove)
                                    {
                                        devicesRemove.Add(GXAmiDevice);
                                    }
                                    else if (it.Action == Actions.State)
                                    {
                                        deviceStates.Add(GXAmiDevice);
                                    }
                                    if (it.Parameters != null)
                                    {
                                        deviceGroup = it.Parameters[0] as GXAmiDeviceGroup;
                                    }
                                }
                                else if (it.Data is GXAmiDeviceGroup)
                                {
                                    GXAmiDeviceGroup dg = it.Data as GXAmiDeviceGroup;
                                    if (it.Action == Actions.Add)
                                    {
                                        deviceGroupsAdd.Add(dg);
                                    }
                                    else if (it.Action == Actions.Edit)
                                    {
                                        deviceGroupsEdit.Add(dg);
                                    }
                                    else if (it.Action == Actions.Remove)
                                    {
                                        deviceGroupsRemove.Add(dg);
                                    }
                                    if (it.Parameters != null)
                                    {
                                        deviceGroupGroup = it.Parameters[0] as GXAmiDeviceGroup;
                                    }
                                }
                                else if (it.Data is GXAmiTask)
                                {
                                    GXAmiTask task = it.Data as GXAmiTask;
                                    if (it.Action == Actions.Add)
                                    {
                                        tasksAdd.Add(task);
                                    }
                                    else if (it.Action == Actions.Edit)
                                    {
                                        tasksClaimed.Add(task);
                                    }
                                    else if (it.Action == Actions.Remove)
                                    {
                                        tasksRemove.Add(task);
                                    }
                                }
                                else if (it.Data is GXAmiTrace)
                                {
                                    GXAmiTrace trace = it.Data as GXAmiTrace;
                                    if (it.Action == Actions.Add)
                                    {
                                        traceAdd.Add(trace);
                                    }
                                    else if (it.Action == Actions.Remove)
                                    {                                        
                                        //TODO: tasksRemove.Add(task);
                                    }
                                }
                                else if (it.Data is GXAmiDataCollector)
                                {
                                    GXAmiDataCollector dc = it.Data as GXAmiDataCollector;
                                    if (it.Action == Actions.Add)
                                    {
                                        dcsAdd.Add(dc);
                                    }
                                    else if (it.Action == Actions.Edit)
                                    {
                                        dcsEdit.Add(dc);
                                    }
                                    else if (it.Action == Actions.Remove)
                                    {
                                        dcsRemove.Add(dc);
                                    }
                                    else if (it.Action == Actions.State)
                                    {
                                        dcStates.Add(dc);
                                    }
                                }
                                else if (it.Data is GXAmiDataRow)
                                {
                                    GXAmiDataRow v = it.Data as GXAmiDataRow;
                                    if (it.Action == Actions.Add)
                                    {
                                        tableRows.Add(v);
                                    }
                                    else if (it.Action == Actions.Edit)
                                    {
                                        //TODO: Table rows cant' edit at the moment. tableRowsEdit.Add(v);
                                    }
                                    else if (it.Action == Actions.Remove)
                                    {
                                        //TODO: Table rows cant' remove at the moment. tableRowsRemove.Add(v);
                                    }
                                }
                                else if (it.Data is GXAmiDataValue)
                                {
                                    GXAmiDataValue v = it.Data as GXAmiDataValue;
                                    if (it.Action == Actions.Add)
                                    {
                                        values.Add(v);
                                    }
                                }
                                else if (it.Data is GXAmiDeviceError)
                                {
                                    GXAmiDeviceError err = it.Data as GXAmiDeviceError;
                                    if (it.Action == Actions.Add)
                                    {
                                        deviceErrorAdd.Add(err);
                                    }
                                    else if (it.Action == Actions.Remove)
                                    {
                                        deviceErrorRemove.Add(err);
                                    }
                                }
                            }
                            catch (System.Net.WebException ex)
                            {
                                if (ex.Status == System.Net.WebExceptionStatus.ConnectionClosed)
                                {
                                    //Connection closed. Ignore and try again...
                                    continue;
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine(ex.Message);
                                if (cl.m_DeviceErrorsAdded != null)
                                {
                                    //TODO: Report error.
                                    //                        GXAmiDeviceError tmp = new GXAmiDeviceError(it.t);
                                    //                      cl.m_DeviceErrorsAdded(cl, new GXAmiDeviceError[] { tmp });
                                }
                            }
                        }
                        try
                        {
                            if (cl.m_UserAdded != null && usersAdd.Count != 0)
                            {
                                cl.m_UserAdded(cl, usersAdd.ToArray());
                                userGroupsEdit.Clear();
                            }
                            if (cl.m_UsersUpdated != null && usersEdit.Count != 0)
                            {
                                cl.m_UsersUpdated(cl, usersEdit.ToArray());
                                usersEdit.Clear();
                            }
                            if (cl.m_UsersRemoved != null && usersRemove.Count != 0)
                            {
                                cl.m_UsersRemoved(cl, usersRemove.ToArray());
                                usersRemove.Clear();
                            }
                            if (cl.m_UserGroupsAdded != null && userGroupsAdd.Count != 0)
                            {
                                cl.m_UserGroupsAdded(cl, userGroupsAdd.ToArray());
                                userGroupsAdd.Clear();
                            }
                            if (cl.m_UserGroupsUpdated != null && userGroupsEdit.Count != 0)
                            {
                                cl.m_UserGroupsUpdated(cl, userGroupsEdit.ToArray());
                                userGroupsEdit.Clear();
                            }
                            if (cl.m_UserGroupsRemoved != null && userGroupsRemove.Count != 0)
                            {
                                cl.m_UserGroupsRemoved(cl, userGroupsRemove.ToArray());
                                userGroupsRemove.Clear();
                            }
                            if (cl.m_DeviceTemplatesAdded != null && deviceTemplatesAdd.Count != 0)
                            {
                                cl.m_DeviceTemplatesAdded(cl, deviceTemplatesAdd.ToArray());
                                deviceTemplatesAdd.Clear();
                            }
                            if (cl.m_DeviceTemplatesRemoved != null && deviceTemplatesRemove.Count != 0)
                            {
                                cl.m_DeviceTemplatesRemoved(cl, deviceTemplatesRemove.ToArray());
                                deviceTemplatesRemove.Clear();
                            }
                            if (cl.m_DevicesAdded != null && devicesAdd.Count != 0)
                            {
                                cl.m_DevicesAdded(cl, deviceGroup, devicesAdd.ToArray());
                                devicesAdd.Clear();
                            }
                            if (cl.m_DevicesUpdated != null && devicesEdit.Count != 0)
                            {
                                cl.m_DevicesUpdated(cl, devicesEdit.ToArray());
                                devicesEdit.Clear();
                            }
                            if (cl.m_DevicesRemoved != null && devicesRemove.Count != 0)
                            {
                                cl.m_DevicesRemoved(cl, devicesRemove.ToArray());
                                devicesRemove.Clear();
                            }
                            if (cl.m_DeviceGroupsAdded != null && deviceGroupsAdd.Count != 0)
                            {
                                cl.m_DeviceGroupsAdded(cl, deviceGroupsAdd.ToArray(), deviceGroupGroup);
                                deviceGroupsAdd.Clear();
                            }
                            if (cl.m_DeviceGroupsUpdated != null && deviceGroupsEdit.Count != 0)
                            {
                                cl.m_DeviceGroupsUpdated(cl, deviceGroupsEdit.ToArray());
                                deviceGroupsEdit.Clear();
                            }
                            if (cl.m_DeviceGroupsRemoved != null && deviceGroupsRemove.Count != 0)
                            {
                                cl.m_DeviceGroupsRemoved(cl, deviceGroupsRemove.ToArray());
                                deviceGroupsRemove.Clear();
                            }
                            if (cl.m_TasksAdded != null && tasksAdd.Count != 0)
                            {
                                cl.m_TasksAdded(cl, tasksAdd.ToArray());
                                tasksAdd.Clear();
                            }
                            if (cl.m_TasksClaimed != null && tasksClaimed.Count != 0)
                            {
                                cl.m_TasksClaimed(cl, tasksClaimed.ToArray());
                                tasksClaimed.Clear();
                            }
                            if (cl.m_TasksRemoved != null && tasksRemove.Count != 0)
                            {
                                cl.m_TasksRemoved(cl, tasksRemove.ToArray());
                                tasksRemove.Clear();
                            }
                            if (cl.m_DeviceErrorsAdded != null && deviceErrorAdd.Count != 0)
                            {
                                cl.m_DeviceErrorsAdded(cl, deviceErrorAdd.ToArray());
                                deviceErrorAdd.Clear();
                            }
                            if (cl.m_DeviceErrorsRemoved != null && deviceErrorRemove.Count != 0)
                            {
                                cl.m_DeviceErrorsRemoved(cl, deviceErrorRemove.ToArray());
                                deviceErrorRemove.Clear();
                            }
                            if (cl.m_SystemErrorsAdded != null)
                            {
                            }
                            if (cl.m_SystemErrorsRemoved != null)
                            {
                            }
                            if (cl.m_ValueUpdated != null && values.Count != 0)
                            {
                                cl.m_ValueUpdated(cl, values.ToArray());
                                values.Clear();
                            }
                            if (cl.m_TableValuesUpdated != null && tableRows.Count != 0)
                            {
                                uint startRow;
                                ulong tableId;
                                object[][] tmp = ItemsToRows(tableRows.ToArray(), out tableId, out startRow);
                                cl.m_TableValuesUpdated(cl, tableId, startRow, tmp);
                                tableRows.Clear();
                            }

                            if (cl.m_DeviceStateChanged != null && deviceStates.Count != 0)
                            {
                                cl.m_DeviceStateChanged(cl, deviceStates.ToArray());
                                deviceStates.Clear();
                            }
                            if (cl.m_DataCollectorStateChanged != null && dcStates.Count != 0)
                            {
                                cl.m_DataCollectorStateChanged(cl, dcStates.ToArray());
                                dcStates.Clear();
                            }
                            if (cl.m_TraceAdded != null && traceAdd.Count != 0)
                            {
                                cl.m_TraceAdded(cl, traceAdd.ToArray());
                                traceAdd.Clear();
                            }
                            if (cl.m_TraceStateChanged != null && traceEdit.Count != 0)
                            {
  //                              cl.m_TraceStateChanged(cl, traceEdit.ToArray());
                                traceEdit.Clear();
                            }
                            if (cl.m_TraceClear != null && traceClear.Count != 0)
                            {
                               //TODO: cl.m_TraceClear(cl, traceClear.ToArray());
                                traceClear.Clear();
                            }


                            if (values.Count != 0)
                            {
                                if (values[0] is GXAmiDataRow)
                                {

                                }
                                else if (cl.m_ValueUpdated != null)
                                {
                                    cl.m_ValueUpdated(cl, values.ToArray());
                                }
                                values.Clear();
                            }
                            if (cl.m_DataCollectorsAdded != null && dcsAdd.Count != 0)
                            {
                                cl.m_DataCollectorsAdded(cl, dcsAdd.ToArray());
                                dcsAdd.Clear();
                            }
                            if (cl.m_DataCollectorsUpdated != null && dcsEdit.Count != 0)
                            {
                                cl.m_DataCollectorsUpdated(cl, dcsEdit.ToArray());
                                dcsEdit.Clear();
                            }
                            if (cl.m_DataCollectorsRemoved != null && dcsRemove.Count != 0)
                            {
                                cl.m_DataCollectorsRemoved(cl, dcsRemove.ToArray());
                                dcsRemove.Clear();
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.Message);
                            if (cl.m_SystemErrorsAdded != null)
                            {
                                //TODO: Report error. cl.m_SystemErrorsAdded(cl, 
                            }
                        }
                    }
                }
            }
            try
            {
                e = new GXEventsRequest(e.ListenerGuid);
                sender.Client.GetAsync<GXEventsResponse>(e,
                    r =>
                    {
                        NotifyEvents(sender, r.Actions, r.DataCollectorGuid, e);
                    }, FailOnAsyncError);
            }
            //This fails sometimes. Just send again.
            catch (System.NullReferenceException)
            {
                e = new GXEventsRequest(e.ListenerGuid);
                sender.Client.GetAsync<GXEventsResponse>(e,
                    r =>
                    {
                        NotifyEvents(sender, r.Actions, r.DataCollectorGuid, e);
                    }, FailOnAsyncError);
            }
        }

		/// <summary>
		/// Data collector Guid.
		/// </summary>
        public Guid DataCollectorGuid
        {
            get;
            set;
        }

        static bool DeviceAddInsLoaded = false;

        static Assembly CurrentDomain_TypeResolve(object sender, ResolveEventArgs args)
        {
            if (!DeviceAddInsLoaded)
            {
                GXDeviceList.Update();
                DeviceAddInsLoaded = true;
            }
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

        /// <summary>
        /// Create device from device template.
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public GXAmiDevice CreateDevice(GXAmiDeviceTemplate template)
        {
            try
            {
                AppDomain.CurrentDomain.TypeResolve += new ResolveEventHandler(CurrentDomain_TypeResolve);
                GXCreateDeviceRequest req = new GXCreateDeviceRequest(new GXAmiDeviceTemplate[]{template});
                GXCreateDeviceResponse res = Client.Get(req);
                foreach (GXAmiParameter p in res.Devices[0].Parameters)
                {
                    if (p.Type == null && p.TypeAsString != null)
                    {
                        p.Type = Type.GetType(p.TypeAsString);                        
                    }
                    if (p.Value != null && p.Type != null && p.Type != typeof(string))
                    {
                        if (p.Type.IsEnum)
                        {
                            p.Value = Enum.Parse(p.Type, p.Value.ToString());
                        }
                        else
                        {
                            p.Value = Convert.ChangeType(p.Value, p.Type);
                        }
                    }
                }
                return res.Devices[0];
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
            finally
            {
                AppDomain.CurrentDomain.TypeResolve -= new ResolveEventHandler(CurrentDomain_TypeResolve);
            }
        }        
        
        public GXAmiDevice GetDevice(ulong deviceId)
        {
             try
            {
                GXDeviceGetRequest req = new GXDeviceGetRequest(deviceId);
                GXDeviceGetResponse res = Client.Get(req);                
                return res.Devices[0];
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }                        
        }
       
        /// <summary>
        /// Datacollector adds itself.
        /// </summary>
        public GXAmiDataCollector RegisterDataCollector()
        {
            try
            {
                GXDataCollectorUpdateRequest req = new GXDataCollectorUpdateRequest(GetMACAddress());
                GXDataCollectorUpdateResponse res = Client.Put(req);                
                if (DataCollectorGuid == Guid.Empty)
                {
                    DataCollectorGuid = res.Collectors[0].Guid;
                    Client.SetCredentials(DataCollectorGuid.ToString(), DataCollectorGuid.ToString());
                }
                return res.Collectors[0];
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Construction.
        /// </summary>
        /// <param name="baseUr">GuruxAMI server address. Example: http://127.0.0.1:1337/</param>
        /// /// <param name="datacollectorGuid">Guid of Data collector.</param>
        public GXAmiClient(string baseUr, Guid datacollectorGuid)
        {
            if (string.IsNullOrEmpty(baseUr))
            {
                throw new ArgumentException();
            }
            try
            {
                Client = new JsonServiceClient(baseUr);
                DataCollectorGuid = datacollectorGuid;
                Client.SetCredentials(datacollectorGuid.ToString(), Guid.NewGuid().ToString());
                Client.AlwaysSendBasicAuthHeader = true;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
            }
        }   

        /// <summary>
        /// Construction.
        /// </summary>
        /// <param name="baseUr">GuruxAMI server address. Example: http://127.0.0.1/GuruxAMI/</param>
        public GXAmiClient(string baseUr, string userName, string password)
        {
            if (string.IsNullOrEmpty(baseUr))
            {
                throw new ArgumentException("Invalid url");
            }
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("Invalid username");
            }
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Invalid password");
            }
            try
            {
                Client = new JsonServiceClient(baseUr);
                Client.SetCredentials(userName, GXAmiUser.GetCryptedPassword(userName, password));
                Client.AlwaysSendBasicAuthHeader = true;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
            }
        }

        /// <summary>
        /// DeviceTypes is the collection of device types that are registered to the computer.
        /// </summary>
        /// <remarks>
        /// With the DeviceTypes collection you can easily figure out, what device type templates are registered to the computer.
        /// With GXPublisher you can easily see registered device types. If the name of the protocol is empty,
        /// all the protocols are returned, otherwise only the ones under it.
        /// </remarks>
        /// <param name="protocol">The name of the protocol.</param>
        /// <param name="removed">Are removed device templates also returned.</param>        
        /// <returns>Collection of device types.</returns>         
        public GXAmiDeviceTemplate[] GetDeviceTypes(bool preset, string protocol, bool removed)
        {
            try
            {
                GXDeviceTemplatesRequest req = new GXDeviceTemplatesRequest(preset, protocol, removed);
                GXDeviceTemplatesResponse res = Client.Post(req);
                return res.Templates;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }            
        }

        /// <summary>
        /// Returns available device templates.
        /// </summary>
        public GXAmiDeviceTemplate[] GetDeviceTemplates(bool removed)
        {
            try
            {
                GXDeviceTemplatesRequest req = new GXDeviceTemplatesRequest((GXAmiDevice[])null, removed);
                GXDeviceTemplatesResponse res = Client.Post(req);
                return res.Templates;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Returns available device templates.
        /// </summary>
        public GXAmiDeviceTemplate[] GetDeviceTemplates(GXAmiDevice device, bool removed)
        {
            try
            {
                GXDeviceTemplatesRequest req = new GXDeviceTemplatesRequest(new GXAmiDevice[] { device }, removed);
                GXDeviceTemplatesResponse res = Client.Post(req);
                return res.Templates;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Returns available device templates.
        /// </summary>
        public GXAmiDeviceTemplate[] GetDeviceTemplates(GXAmiDataCollector collector, bool removed)
        {
            try
            {
                GXDeviceTemplatesRequest req = new GXDeviceTemplatesRequest(new GXAmiDataCollector[] { collector }, removed);
                GXDeviceTemplatesResponse res = Client.Post(req);
                return res.Templates;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Returns device templates for the device.
        /// </summary>
        public byte[] GetDeviceTemplateData(ulong deviceId)
        {
            try
            {
                GXDeviceTemplateDataRequest req = new GXDeviceTemplateDataRequest(deviceId);
                GXDeviceTemplateDataResponse res = Client.Post(req);
                return res.Data;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Returns device templates for the device.
        /// </summary>
        public byte[] GetDeviceTemplateData(Guid DevicetemplateGuid)
        {
            try
            {
                GXDeviceTemplateDataRequest req = new GXDeviceTemplateDataRequest(DevicetemplateGuid);
                GXDeviceTemplateDataResponse res = Client.Post(req);
                return res.Data;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get systems errors.
        /// </summary>
        /// <returns>Collection of system errors.</returns>
        public GXAmiSystemError[] GetSystemErrors()
        {
            try
            {
                GXErrorsRequest req = new GXErrorsRequest(true);
                GXErrorsResponse res = Client.Post(req);
                return res.SystemErrors;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get systems errors.
        /// </summary>
        /// <returns>Collection of system errors.</returns>
        public GXAmiSystemError[] GetSystemErrors(GXAmiUser user)
        {
            try
            {
                GXErrorsRequest req = new GXErrorsRequest(true, user);
                GXErrorsResponse res = Client.Post(req);
                return res.SystemErrors;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }
        
        /// <summary>
        // Add schedules.
        /// </summary>
        /// <returns></returns>
        public void AddSchedules(GXAmiSchedule[] schedules)
        {
            try
            {
                GXScheduleUpdateRequest req = new GXScheduleUpdateRequest(schedules);
                GXScheduleUpdateResponse res = Client.Post(req);
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
            }
        }

        /// <summary>
        // Get all schedules.
        /// </summary>
        /// <returns></returns>
        public GXAmiSchedule[] GetSchedules()
        {
            try
            {
                GXScheduleRequest req = new GXScheduleRequest();
                GXScheduleResponse res = Client.Post(req);
                return res.Schedules;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        // Get schedules of the user.
        /// </summary>
        /// <returns></returns>
        public GXAmiSchedule[] GetSchedules(GXAmiUser user)
        {
            try
            {
                GXScheduleRequest req = new GXScheduleRequest(new GXAmiUser[] { user });
                GXScheduleResponse res = Client.Post(req);
                return res.Schedules;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        // Get schedules of the user group.
        /// </summary>
        /// <returns></returns>
        public GXAmiSchedule[] GetSchedules(GXAmiUserGroup group)
        {
            try
            {
                GXScheduleRequest req = new GXScheduleRequest(new GXAmiUserGroup[] { group });
                GXScheduleResponse res = Client.Post(req);
                return res.Schedules;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        // Get all device errors.
        /// </summary>
        /// <returns></returns>
        public GXAmiDeviceError[] GetErrors()
        {
            try
            {
                GXErrorsRequest req = new GXErrorsRequest(false);
                GXErrorsResponse res = Client.Post(req);
                return res.DeviceErrors;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        // Get all user device errors.
        /// </summary>
        /// <returns></returns>
        public GXAmiDeviceError[] GetErrors(GXAmiUser user)
        {
            try
            {
                GXErrorsRequest req = new GXErrorsRequest(false, user);
                GXErrorsResponse res = Client.Post(req);
                return res.DeviceErrors;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        // Get all user group device errors.
        /// </summary>
        /// <returns></returns>
        public GXAmiDeviceError[] GetErrors(GXAmiUserGroup group)
        {
            try
            {
                GXErrorsRequest req = new GXErrorsRequest(group);
                GXErrorsResponse res = Client.Post(req);
                return res.DeviceErrors;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        // Get all device group errors.
        /// </summary>
        /// <returns></returns>
        public GXAmiDeviceError[] GetErrors(GXAmiDeviceGroup group)
        {
            try
            {
                GXErrorsRequest req = new GXErrorsRequest(group);
                GXErrorsResponse res = Client.Post(req);
                return res.DeviceErrors;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        // Get all device errors.
        /// </summary>
        /// <returns></returns>
        public GXAmiDeviceError[] GetErrors(GXAmiDevice device)
        {
            try
            {
                GXErrorsRequest req = new GXErrorsRequest(device);
                GXErrorsResponse res = Client.Post(req);
                return res.DeviceErrors;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }
        
        /// <summary>
        /// Remove selected task.
        /// </summary>
        /// <remarks>
        /// DC removes task after it is compleated.
        /// </remarks>
        /// <param name="permamently">Is item removed permamently.</param>
        public void RemoveTask(GXAmiTask task, bool permamently)
        {
			try
			{				
				GXTaskDeleteRequest req = new GXTaskDeleteRequest(new GXAmiTask[] { task });
				GXTaskDeleteResponse res = Client.Delete(req);
			}
			catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
			{
				ThrowException(ex);
			}			
        }

        /// <summary>
        /// Remove selected task.
        /// </summary>
        /// <remarks>
        /// DC removes task after it is compleated.
        /// </remarks>
        /// <param name="permamently">Is item removed permamently.</param>
        public void RemoveTask(GXAmiTask[] tasks, bool permamently)
        {
            try
            {
                GXTaskDeleteRequest req = new GXTaskDeleteRequest(tasks);
                GXTaskDeleteResponse res = Client.Delete(req);
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
            }
        }

        /// <summary>
        /// Claim selected task.
        /// </summary>
        /// <returns>Returns info from the claimed task. If claim failed it is null.</returns>
        public GXClaimedTask ClaimTask(GXAmiTask task)
        {
            try
            {
                GXTasksClaimRequest req = new GXTasksClaimRequest(new GXAmiTask[] { task });
                GXTasksClaimResponse res = Client.Post(req);
                if (res.Tasks.Length != 1)
                {
                    return null;
                }
                task.DataCollectorGuid = this.DataCollectorGuid;
                res.Tasks[0].Task = task;
                return res.Tasks[0];
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get all tasks.
        /// </summary>
        /// <returns></returns>
        /// <param name="state">State of task.</param>
        public GXAmiTask[] GetTasks(TaskState state)
        {
            try
            {
                GXTasksRequest req = new GXTasksRequest(state);
                GXTasksResponse res = Client.Post(req);
                return res.Tasks;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get task by task id.
        /// </summary>
        /// <returns></returns>
        /// <param name="taskId">Task ID.</param>
        /// <param name="log">Is item search from log table.</param>
        public GXAmiTask GetTaskById(ulong taskId, bool log)
        {
            try
            {
                GXTasksRequest req = new GXTasksRequest(new ulong[] { taskId }, log);
                GXTasksResponse res = Client.Post(req);
                if (res.Tasks.Length != 1)
                {
                    return null;
                }
                return res.Tasks[0];
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }
        
        /// <summary>
        /// Update device state.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="status"></param>
        public void UpdateDeviceState(ulong deviceId, DeviceStates status)
        {
            try
            {
                Dictionary<ulong, DeviceStates> states = new Dictionary<ulong,DeviceStates>();
                states.Add(deviceId, status);
                GXDeviceStateUpdateRequest req = new GXDeviceStateUpdateRequest(states);
                Client.Put(req);                
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);                
            }
        }

        /// <summary>
        /// Update Data Collector state.
        /// </summary>
        /// <param name="dataCollectorId"></param>
        /// <param name="status"></param>
        public void UpdateDataCollectorState(ulong dataCollectorId, DeviceStates status)
        {
            try
            {
                Dictionary<ulong, DeviceStates> states = new Dictionary<ulong, DeviceStates>();
                states.Add(dataCollectorId, status);
                GXDataCollectorStateUpdateRequest req = new GXDataCollectorStateUpdateRequest(states);
                Client.Put(req);
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
            }
        }

        /// <summary>
        // Get all user tasks.
        /// </summary>
        /// <returns></returns>
        /// <param name="state">State of task.</param>
        public GXAmiTask[] GetTasks(GXAmiUser user, TaskState state)
        {
            try
            {
                GXTasksRequest req = new GXTasksRequest(state, new GXAmiUser[] { user });
                GXTasksResponse res = Client.Post(req);
                return res.Tasks;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        // Get all user group tasks.
        /// </summary>
        /// <returns></returns>
        /// <param name="state">State of task.</param>
        public GXAmiTask[] GetTasks(GXAmiUserGroup group, TaskState state)
        {
            try
            {
                GXTasksRequest req = new GXTasksRequest(state, new GXAmiUserGroup[] { group });
                GXTasksResponse res = Client.Post(req);
                return res.Tasks;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        // Get all device group tasks.
        /// </summary>
        /// <returns></returns>
        /// <param name="state">State of task.</param>
        public GXAmiTask[] GetTasks(GXAmiDeviceGroup group, TaskState state)
        {
            try
            {
                GXTasksRequest req = new GXTasksRequest(state, new GXAmiDeviceGroup[] { group });
                GXTasksResponse res = Client.Post(req);
                return res.Tasks;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        // Get all device tasks.
        /// </summary>
        /// <param name="state">State of task.</param>
        /// <returns></returns>
        public GXAmiTask[] GetTasks(GXAmiDevice device, TaskState state)
        {
            try
            {
                GXTasksRequest req = new GXTasksRequest(state, new GXAmiDevice[] { device });
                GXTasksResponse res = Client.Post(req);
                return res.Tasks;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get all actions.
        /// </summary>
        /// <returns></returns>
        public GXAmiUserActionLog[] GetActions()
        {
            try
            {
                GXActionRequest req = new GXActionRequest();
                GXActionResponse res = Client.Post(req);
                return res.Actions;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        // Get all user actions.
        /// </summary>
        /// <returns></returns>
        public GXAmiUserActionLog[] GetActions(GXAmiUser user)
        {
            try
            {
                GXActionRequest req = new GXActionRequest(user);
                GXActionResponse res = Client.Post(req);
                return res.Actions;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        // Get all user group actions.
        /// </summary>
        /// <returns></returns>
        public GXAmiUserActionLog[] GetActions(GXAmiUserGroup group)
        {
            try
            {
                GXActionRequest req = new GXActionRequest(group);
                GXActionResponse res = Client.Post(req);
                return res.Actions;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        // Get all device group actions.
        /// </summary>
        /// <returns></returns>
        public GXAmiUserActionLog[] GetActions(GXAmiDeviceGroup group)
        {
            try
            {
                GXActionRequest req = new GXActionRequest(group);
                GXActionResponse res = Client.Post(req);
                return res.Actions;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        // Get all device actions.
        /// </summary>
        /// <returns></returns>
        public GXAmiUserActionLog[] GetActions(GXAmiDevice device)
        {
            try
            {
                GXActionRequest req = new GXActionRequest(device);
                GXActionResponse res = Client.Post(req);
                return res.Actions;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get all users.
        /// </summary>
        /// <returns></returns>
        public GXAmiUser[] GetUsers(bool removed)
        {
            return GetUsers(null, removed);
        }

        /// <summary>
        /// Convert received exception to normal exception.
        /// </summary>
        /// <param name="ex"></param>
        void ThrowException(ServiceStack.ServiceClient.Web.WebServiceException ex)
        {
            if (ex.Message == "ArgumentException")
            {
                throw new ArgumentException(ex.ErrorMessage);
            }
            if (ex.Message == "ArgumentNullException")
            {
                throw new ArgumentNullException(ex.ErrorMessage);
            }
            if (ex.ErrorCode == "MySqlException")
            {
                throw new Exception(ex.ErrorMessage);
            }
            if (ex.ErrorCode == "Unauthorized")
            {
                throw new UnauthorizedAccessException();
            }
            if (ex.ErrorMessage == null)
            {
                throw new Exception(ex.StatusDescription);
            }
            throw new Exception(ex.ErrorMessage);
        }

        /// <summary>
        /// Get Users from the user group.
        /// </summary>
        /// <returns></returns>
        public GXAmiUser[] GetUsers(GXAmiUserGroup group, bool removed)
        {
            try
            {
                GXUsersRequest req = new GXUsersRequest(group);
                req.Removed = removed;
                GXUsersResponse res = Client.Post(req);
                return res.Users;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get all user groups.
        /// </summary>
        /// <returns></returns>
        public GXAmiUserGroup[] GetUserGroups(bool removed)
        {
            return GetUserGroups((GXAmiDeviceGroup)null, removed);
        }

        /// <summary>
        /// Get user groups where the device group belongs.
        /// </summary>
        /// <returns></returns>
        public GXAmiUserGroup[] GetUserGroups(GXAmiDeviceGroup group, bool removed)
        {
            try
            {
                GXUserGroupsRequest req = new GXUserGroupsRequest(group);
                req.Removed = removed;
                GXUserGroupResponse res = Client.Post(req);
                return res.UserGroups;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get user groups where the device belongs.
        /// </summary>
        /// <returns></returns>
        public GXAmiUserGroup[] GetUserGroups(GXAmiDevice device, bool removed)
        {
            try
            {
                GXUserGroupsRequest req = new GXUserGroupsRequest(device);
                req.Removed = removed;
                GXUserGroupResponse res = Client.Post(req);
                return res.UserGroups;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get user groups where the user belongs.
        /// </summary>
        /// <returns></returns>
        public GXAmiUserGroup[] GetUserGroups(GXAmiUser user, bool removed)
        {
            try
            {
                GXUserGroupsRequest req = new GXUserGroupsRequest(user);
                req.Removed = removed;
                GXUserGroupResponse res = Client.Post(req);
                return res.UserGroups;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Returns device content also (Categories, tables and properties).
        /// </summary>
        /// <returns></returns>
        public GXAmiDevice[] GetDeviceContent(GXAmiDevice device)
        {
            try
            {
                GXDevicesRequest req = new GXDevicesRequest(device, true);
                GXDevicesResponse res = Client.Post(req);
                return res.Devices;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get all devices.
        /// </summary>
        /// <returns></returns>
        public GXAmiDevice[] GetDevices(bool removed)
        {
            try
            {
                GXDevicesRequest req = new GXDevicesRequest();
                req.Removed = removed;
                GXDevicesResponse res = Client.Post(req);
                return res.Devices;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get devices from the device group.
        /// </summary>
        /// <returns></returns>
        public GXAmiDevice[] GetDevices(GXAmiDeviceGroup group, bool removed)
        {
            try
            {
                GXDevicesRequest req = new GXDevicesRequest(group);
                req.Removed = removed;
                GXDevicesResponse res = Client.Post(req);
                return res.Devices;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get devices from the data collector.
        /// </summary>
        /// <returns></returns>
        public GXAmiDevice[] GetDevices(GXAmiDataCollector collector, bool content, bool removed)
        {
            try
            {
                GXDevicesRequest req = new GXDevicesRequest(collector);
                req.Removed = removed;
                req.Content = content;
                GXDevicesResponse res = Client.Post(req);
                return res.Devices;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get user group devices.
        /// </summary>
        /// <returns></returns>
        public GXAmiDevice[] GetDevices(GXAmiUserGroup group, bool removed)
        {
            try
            {
                GXDevicesRequest req = new GXDevicesRequest(group);
                req.Removed = removed;
                GXDevicesResponse res = Client.Post(req);
                return res.Devices;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get devices of the user.
        /// </summary>
        /// <returns></returns>
        public GXAmiDevice[] GetDevices(GXAmiUser user, bool removed)
        {
            try
            {
                GXDevicesRequest req = new GXDevicesRequest(user);
                req.Removed = removed;
                GXDevicesResponse res = Client.Post(req);
                return res.Devices;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get all device groups.
        /// </summary>
        /// <returns></returns>
        public GXAmiDeviceGroup[] GetDeviceGroups(bool removed)
        {
            try
            {
                GXDeviceGroupsRequest req = new GXDeviceGroupsRequest();
                req.Removed = removed;
                GXDeviceGroupResponse res = Client.Post(req);
                return res.DeviceGroups;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get device groups from the device group.
        /// </summary>
        /// <returns></returns>
        public GXAmiDeviceGroup[] GetDeviceGroups(GXAmiDeviceGroup group, bool removed)
        {
            try
            {
                GXDeviceGroupsRequest req = new GXDeviceGroupsRequest(group);
                req.Removed = removed;
                GXDeviceGroupResponse res = Client.Post(req);
                return res.DeviceGroups;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get device groups of the user group.
        /// </summary>
        /// <returns></returns>
        public GXAmiDeviceGroup[] GetDeviceGroups(GXAmiUserGroup group, bool removed)
        {
            try
            {
                GXDeviceGroupsRequest req = new GXDeviceGroupsRequest(group);
                req.Removed = removed;
                GXDeviceGroupResponse res = Client.Post(req);
                return res.DeviceGroups;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get device groups of the user.
        /// </summary>
        /// <returns></returns>
        public GXAmiDeviceGroup[] GetDeviceGroups(GXAmiUser user, bool removed)
        {
            try
            {
                GXDeviceGroupsRequest req = new GXDeviceGroupsRequest(user);
                req.Removed = removed;
                GXDeviceGroupResponse res = Client.Post(req);
                return res.DeviceGroups;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Remove user.
        /// </summary>
        /// <param name="user">User to remove.</param>
        /// <param name="permamently">Is item removed permamently.</param>        
        public void RemoveUser(GXAmiUser user, bool permamently)
        {
            try
            {
                GXUserDeleteRequest req = new GXUserDeleteRequest(new GXAmiUser[] { user }, permamently);
                GXUserDeleteResponse res = Client.Delete(req);
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
            }
        }

        /// <summary>
        /// Remove user group.
        /// </summary>
        /// <param name="group">User group to remove.</param>
        /// <param name="permamently">Is item removed permamently.</param>        
        public void RemoveUserGroup(GXAmiUserGroup group, bool permamently)
        {
            try
            {
                GXUserGroupDeleteRequest req = new GXUserGroupDeleteRequest(new GXAmiUserGroup[] { group }, permamently);
                GXUserGroupDeleteResponse res = Client.Delete(req);
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
            }
        }

        /// <summary>
        /// Remove device group.
        /// </summary>
        /// <param name="target">GXAmiDevice group to remove.</param>
        /// <param name="permamently">Is item removed permamently.</param>        
        public void RemoveDeviceGroup(GXAmiDeviceGroup target, bool permamently)
        {
            try
            {
                GXDeviceGroupDeleteRequest req = new GXDeviceGroupDeleteRequest(new GXAmiDeviceGroup[] { target as GXAmiDeviceGroup }, permamently);
                GXDeviceGroupDeleteResponse res = Client.Delete(req);
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
            }

        }

        /// <summary>
        /// Remove device.
        /// </summary>
        /// <param name="target">GXAmiDevice to remove.</param>
        /// <param name="permamently">Is item removed permamently.</param>        
        public void RemoveDevice(GXAmiDevice target, bool permamently)
        {
            try
            {
                GXDeviceDeleteRequest req = new GXDeviceDeleteRequest(new GXAmiDevice[] { target }, permamently);
                GXDeviceDeleteResponse res = Client.Delete(req);
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
            }
        }

        /// <summary>
        /// Remove device.
        /// </summary>
        /// <param name="device">GXAmiDevice to remove.</param>
        /// <param name="deviceGroup">GXAmiDevice group where device is removed.</param>
        /// <param name="permamently">Is item removed permamently.</param>        
        public void RemoveDevice(GXAmiDevice device, GXAmiDeviceGroup deviceGroup, bool permamently)
        {
            try
            {
                GXDeviceDeleteRequest req = new GXDeviceDeleteRequest(new GXAmiDevice[] { device }, new GXAmiDeviceGroup[] { deviceGroup }, permamently);
                GXDeviceDeleteResponse res = Client.Delete(req);
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
            }

        }


        /// <summary>
        /// Remove selected item.
        /// </summary>
        /// <param name="target">Removed item.</param>
        /// <param name="permamently">Is item removed permamently.</param>        
        void Remove(object target, bool permamently)
        {
            try
            {
                if (target is GXAmiUserActionLog)
                {
                    GXActionDeleteRequest req = new GXActionDeleteRequest(new GXAmiUserActionLog[] { target as GXAmiUserActionLog });
                    GXActionDeleteResponse res = Client.Delete(req);
                    return;
                }
                if (target is GXAmiTask)
                {
                    GXTaskDeleteRequest req = new GXTaskDeleteRequest(new GXAmiTask[] { target as GXAmiTask });
                    GXTaskDeleteResponse res = Client.Delete(req);
                    return;
                }
                if (target is GXAmiSystemError)
                {
                    GXErrorDeleteRequest req = new GXErrorDeleteRequest(new GXAmiSystemError[] { target as GXAmiSystemError });
                    GXErrorDeleteResponse res = Client.Delete(req);
                    return;
                }
                if (target is GXAmiDeviceError)
                {
                    GXErrorDeleteRequest req = new GXErrorDeleteRequest(new GXAmiDeviceError[] { target as GXAmiDeviceError });
                    GXErrorDeleteResponse res = Client.Delete(req);
                    return;
                }
                throw new Exception("Invalid target");
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
            }
        }

        /// <summary>
        /// Add new user to the user group.
        /// </summary>
        /// <param name="user">Added user.</param>
        /// <param name="group">Target user group.</param>
        public void AddUser(GXAmiUser user, GXAmiUserGroup group)
        {
            try
            {
                GXUserUpdateRequest req = new GXUserUpdateRequest(Actions.Add, new GXAmiUser[] { user }, new GXAmiUserGroup[] { group });
                GXUserUpdateResponse res = Client.Put(req);
                user.Id = res.Users[0].Id;
                group.Id = res.UserGroups[0].Id;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
            }
        }

        /// <summary>
        /// Add new device to the device group.
        /// </summary>
        /// <param name="device">Added device.</param>
        /// <param name="group">Target device group.</param>
        public void AddDevice(GXAmiDevice device, GXAmiDeviceGroup[] groups)
        {
            if (device == null || groups == null)
            {
                throw new ArgumentNullException();
            }
            foreach (GXAmiDeviceGroup it in groups)
            {
                if (it.Id == 0)
                {
                    throw new ArgumentOutOfRangeException("Device Group is not added when device is added to the group.");
                }
            }
            try
            {
                GXDeviceUpdateRequest req = new GXDeviceUpdateRequest(Actions.Add, new GXAmiDevice[] { device }, groups);
                GXDeviceUpdateResponse res = Client.Put(req);
                device.Id = res.Devices[0].Id;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
            }
        }       

        public static byte[] GetMACAddress()
        {
            NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nis)
            {
                if (adapter.OperationalStatus == OperationalStatus.Up)
                {
                    return adapter.GetPhysicalAddress().GetAddressBytes();
                }
            }
            return null;
        }

        public static string GetMACAddressAsString()
        {
            return BitConverter.ToString(GetMACAddress()).Replace('-', ':');
        }


        byte[] GetMacAddress(string mac)
        {
            return Gurux.Common.GXCommon.HexToBytes(mac.Replace(":", ""), false);
        }

        /// <summary>
        /// Remove selected datacollectors.
        /// </summary>        
        public void RemoveDataCollectors(GXAmiDataCollector[] datacollectors, bool permamently)
        {
            try
            {
                GXDataCollectorDeleteRequest req = new GXDataCollectorDeleteRequest(datacollectors, permamently);
                Client.Delete(req);                
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);                
            }
        }

        /// <summary>
        /// Datacollector adds itself.
        /// </summary>
        /// <param name="mac">MAC Address of data collector. Address must be divided by 2.</param>
        public GXAmiDataCollector AddDataCollector(string mac)
        {
            try
            {
                GXDataCollectorUpdateRequest req = new GXDataCollectorUpdateRequest(GetMacAddress(mac));
                GXDataCollectorUpdateResponse res = Client.Put(req);
                return res.Collectors[0];
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }


        /// <summary>
        /// Add new data collector to the user groups.
        /// </summary>
        /// <param name="device">Added data collector.</param>
        /// <param name="group">Target user groups.</param>
        public void AddDataCollector(GXAmiDataCollector datacollector, GXAmiUserGroup[] userGroups)
        {
            try
            {
                GXDataCollectorUpdateRequest req = new GXDataCollectorUpdateRequest(new GXAmiDataCollector[] { datacollector }, userGroups);
                GXDataCollectorUpdateResponse res = Client.Put(req);
                datacollector.Id = res.Collectors[0].Id;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
            }
        }

        /// <summary>
        /// Add new data collector to collect data from the device(s).
        /// </summary>
        /// <param name="device">Added data collector.</param>
        /// <param name="group">Target devices.</param>
        public void AddDataCollector(GXAmiDataCollector datacollector, GXAmiDevice[] devices)
        {
            try
            {
                GXDataCollectorUpdateRequest req = new GXDataCollectorUpdateRequest(new GXAmiDataCollector[] { datacollector }, devices);
                GXDataCollectorUpdateResponse res = Client.Put(req);
                datacollector.Id = res.Collectors[0].Id;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
            }
        }

        /// <summary>
        /// Get data collectors that device uses.
        /// </summary>
        /// <param name="device"></param>
        public GXAmiDataCollector[] GetDataCollectors(GXAmiDevice device)
        {
            try
            {
                GXDataCollectorsRequest req = new GXDataCollectorsRequest(device);
                GXDataCollectorsResponse res = Client.Post(req);
                return res.Collectors;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get data collectors that are available for the user.
        /// </summary>
        /// <param name="usergroup"></param>
        public GXAmiDataCollector[] GetDataCollectors(GXAmiUser user)
        {
            try
            {
                GXDataCollectorsRequest req = new GXDataCollectorsRequest(user);
                GXDataCollectorsResponse res = Client.Post(req);
                return res.Collectors;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get data collectors that are available for the user.
        /// </summary>
        /// <param name="usergroup"></param>
        public GXAmiDataCollector[] GetDataCollector(GXAmiUser user)
        {
            try
            {
                GXDataCollectorsRequest req = new GXDataCollectorsRequest(user);
                GXDataCollectorsResponse res = Client.Post(req);
                return res.Collectors;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }       

        /*
        /// <summary>
        /// Get data collectors that are available for the user group.
        /// </summary>
        /// <param name="usergroup"></param>
        public GXAmiDataCollector[] GetDataCollectors(UserGroup usergroup)
        {
            try
            {
                GXDataCollectorsRequest req = new GXDataCollectorsRequest(usergroup);
                GXDataCollectorsResponse res = Client.Post(req);
                return res.Collectors;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }
        */

        /// <summary>
        /// Get data collectors that user can access.
        /// </summary>
        /// <param name="usergroup"></param>
        public GXAmiDataCollector[] GetDataCollectors()
        {
            try
            {
                GXDataCollectorsRequest req = new GXDataCollectorsRequest(false);
                GXDataCollectorsResponse res = Client.Post(req);
                return res.Collectors;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get data collectors by MAC address.
        /// </summary>
        /// <param name="mac">MAC address.</param>
        public GXAmiDataCollector[] GetDataCollectorsByMacAdderss(string mac)
        {
            try
            {
                GXDataCollectorsRequest req = new GXDataCollectorsRequest(GetMacAddress(mac));
                GXDataCollectorsResponse res = Client.Post(req);
                return res.Collectors;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get data collectors by IP address.
        /// </summary>
        /// <param name="ipAddress">IP Address.</param>
        public GXAmiDataCollector[] GetDataCollectorsByIpAdderss(string ipAddress)
        {
            try
            {
                GXDataCollectorsRequest req = new GXDataCollectorsRequest(ipAddress);
                GXDataCollectorsResponse res = Client.Post(req);
                return res.Collectors;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get data collectors by Guid.
        /// </summary>
        /// <param name="guid">Guid.</param>
        public GXAmiDataCollector[] GetDataCollectorsByGuid(Guid guid)
        {
            try
            {
                GXDataCollectorsRequest req = new GXDataCollectorsRequest(guid);
                GXDataCollectorsResponse res = Client.Post(req);
                return res.Collectors;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Gets data collectors that are not assigned for device or user.
        /// </summary>
        public GXAmiDataCollector[] GetUnassignedDataCollectors()
        {
            try
            {
                GXDataCollectorsRequest req = new GXDataCollectorsRequest(true);
                GXDataCollectorsResponse res = Client.Post(req);
                return res.Collectors;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }


        /// <summary>
        /// Add device group to the user group.
        /// </summary>
        /// <param name="deviceGroup">Added device group.</param>
        /// <param name="userGroup">Target user group.</param>
        public void AddDeviceGroup(GXAmiDeviceGroup deviceGroup, GXAmiUserGroup userGroup)
        {
            if (userGroup.Id == 0)
            {
                throw new ArgumentOutOfRangeException("User Group is not added when Device Group is added to the group.");
            }
            try
            {
                GXDeviceGroupUpdateRequest req = new GXDeviceGroupUpdateRequest(Actions.Add, new GXAmiDeviceGroup[] { deviceGroup }, new GXAmiUserGroup[] { userGroup });
                GXDeviceGroupUpdateResponse res = Client.Put(req);
                deviceGroup.Id = res.DeviceGroups[0].Id;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
            }
        }

        /// <summary>
        /// Add new device device to the device group.
        /// </summary>
        /// <param name="device">Added device group.</param>
        /// <param name="group">Target device group.</param>
        public void AddDeviceGroup(GXAmiDeviceGroup target, GXAmiDeviceGroup group)
        {
            if (target == null || group == null)
            {
                throw new ArgumentNullException();
            }
            if (group.Id == 0)
            {
                throw new ArgumentOutOfRangeException("Device Group is not added when device group is added to the group.");
            }
            try
            {
                GXDeviceGroupUpdateRequest req = new GXDeviceGroupUpdateRequest(Actions.Add, new GXAmiDeviceGroup[] { target }, new GXAmiDeviceGroup[] { group });
                GXDeviceGroupUpdateResponse res = Client.Put(req);
                target.Id = res.DeviceGroups[0].Id;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
            }
        }

        /// <summary>
        /// Add new user group.
        /// </summary>
        /// <param name="target">Added user group.</param>
        public void AddUserGroup(GXAmiUserGroup target)
        {
            try
            {
                GXUserGroupUpdateRequest req = new GXUserGroupUpdateRequest(Actions.Add, new GXAmiUserGroup[] { target });
                GXUserGroupUpdateResponse res = Client.Put(req);
                target.Id = res.UserGroups[0].Id;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
            }
        }

        /// <summary>
        /// Remove target from the group.
        /// </summary>
        /// <param name="target">Removed object.</param>
        /// <param name="group">Group where target is removed.</param>
        /// <param name="permamently">Is item removed permamently.</param>        
        public void Remove(object target, object group, bool permamently)
        {
            try
            {
                if (target is GXAmiUser)
                {
                    if (group is GXAmiUserGroup)
                    {
                        GXUserDeleteRequest req = new GXUserDeleteRequest(new GXAmiUser[] { target as GXAmiUser }, new GXAmiUserGroup[] { group as GXAmiUserGroup }, permamently);
                        GXUserDeleteResponse res = Client.Delete(req);
                        return;
                    }
                }
                if (target is GXAmiDevice)
                {
                    if (group is GXAmiDeviceGroup)
                    {
                        GXDeviceDeleteRequest req = new GXDeviceDeleteRequest(new GXAmiDevice[] { target as GXAmiDevice }, new GXAmiDeviceGroup[] { group as GXAmiDeviceGroup }, permamently);
                        GXDeviceDeleteResponse res = Client.Delete(req);
                        return;
                    }
                }
                if (target is GXAmiDeviceGroup)
                {
                    if (group is GXAmiDeviceGroup)
                    {
                        GXDeviceGroupDeleteRequest req = new GXDeviceGroupDeleteRequest(new GXAmiDeviceGroup[] { target as GXAmiDeviceGroup }, new GXAmiDeviceGroup[] { group as GXAmiDeviceGroup }, permamently);
                        GXDeviceGroupDeleteResponse res = Client.Delete(req);
                        return;
                    }
                }
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
            }
            throw new Exception("Invalid target");
        }        

        /// <summary>
        /// Read table by entry (index and count).
        /// </summary>
        /// <param name="target">Table to read.</param>
        /// <param name="index">Zero based start index.</param>
        /// <param name="count">Row count. Zwero if all rows are read.</param>
        /// <returns></returns>
        public GXAmiTask ReadTable(GXAmiDataTable target, int index, int count)
        {
            try
            {
                string[] p = new string[] { ((int)PartialReadType.Entry).ToString(), index.ToString(), count.ToString() };
                GXAmiTask task = new GXAmiTask(TaskType.Read, target);
                task.Data = string.Join(";", p);
                GXTaskUpdateRequest req = new GXTaskUpdateRequest(new GXAmiTask[] { task });
                GXTaskUpdateResponse res = Client.Put(req);
                return res.Tasks[0];
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Read table by range (between start and end days).
        /// </summary>
        /// <param name="target">Table to read.</param>
        /// <param name="index">Zero based start index.</param>
        /// <param name="count">Row count. Zwero if all rows are read.</param>
        /// <returns></returns>
        public GXAmiTask ReadTable(GXAmiDataTable target, DateTime start, DateTime end)
        {
            try
            {
                string[] p = new string[] { ((int)PartialReadType.Range).ToString(), start.ToString(), end.ToString() };
                GXAmiTask task = new GXAmiTask(TaskType.Read, target);
                task.Data = string.Join(";", p);
                GXTaskUpdateRequest req = new GXTaskUpdateRequest(new GXAmiTask[] { task });
                GXTaskUpdateResponse res = Client.Put(req);
                return res.Tasks[0];
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Read selected item.
        /// </summary>
        public GXAmiTask Read(object target)
        {
            try
            {
                GXTaskUpdateRequest req = new GXTaskUpdateRequest(new GXAmiTask[] { new GXAmiTask(TaskType.Read, target) });
                GXTaskUpdateResponse res = Client.Put(req);
                return res.Tasks[0];
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Write selected item.
        /// </summary>
        /// <param name="target"></param>
        public GXAmiTask Write(object target)
        {
            try
            {
                GXTaskUpdateRequest req = new GXTaskUpdateRequest(new GXAmiTask[] { new GXAmiTask(TaskType.Write, target) });
                GXTaskUpdateResponse res = Client.Put(req);
                return res.Tasks[0];
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Open Media of the Data Collector.
        /// </summary>
        /// <param name="target"></param>
        public GXAmiTask MediaOpen(Guid dataCollector, string media, string settings)
        {
            try
            {
                if (settings == null)
                {
                    settings = "";
                }
                string data = media + Environment.NewLine + settings.Replace(Environment.NewLine, "");
                GXTaskUpdateRequest req = new GXTaskUpdateRequest(new GXAmiTask[] { new GXAmiTask(TaskType.MediaOpen, dataCollector, data) });
                GXTaskUpdateResponse res = Client.Put(req);
                return res.Tasks[0];
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Close Media of the Data Collector.
        /// </summary>
        /// <param name="target"></param>
        public GXAmiTask MediaClose(Guid dataCollector, string media, string settings)
        {
            try
            {
                if (settings == null)
                {
                    settings = "";
                }
                string data = media + Environment.NewLine + settings.Replace(Environment.NewLine, "");
                GXTaskUpdateRequest req = new GXTaskUpdateRequest(new GXAmiTask[] { new GXAmiTask(TaskType.MediaClose, dataCollector, data) });
                GXTaskUpdateResponse res = Client.Put(req);
                return res.Tasks[0];
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// DC notifies that media state is changed.
        /// </summary>
        /// <param name="target"></param>
        internal GXAmiTask MediaStateChange(Guid dataCollector, string media, string settings, MediaState state)
        {
            try
            {
                string data = media + Environment.NewLine + settings.Replace(Environment.NewLine, "") + Environment.NewLine + ((int)state).ToString();
                GXTaskUpdateRequest req = new GXTaskUpdateRequest(new GXAmiTask[] { new GXAmiTask(TaskType.MediaState, dataCollector, data) });
                GXTaskUpdateResponse res = Client.Put(req);
                return res.Tasks[0];
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }


        /// <summary>
        /// DC notifies that media error is occurred.
        /// </summary>
        internal void MediaError(GXAmiTask task, Exception ex)
        {
            try
            {
                GXErrorUpdateRequest req = new GXErrorUpdateRequest(task.DataCollectorID, task.Id, 1, ex);
                GXErrorUpdateResponse res = Client.Put(req);
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex2)
            {
                ThrowException(ex2);
            }
        }


        /// <summary>
        /// Write data to the collector.
        /// </summary>
        /// <param name="target"></param>
        public GXAmiTask Write(Guid collectorGuid, string media, string settings, byte[] data, int count, object eop)
        {
            if (data == null || data.Length == 0)
            {
                throw new ArgumentNullException("Invalid Data.");
            }
            if (collectorGuid == Guid.Empty)
            {
                throw new ArgumentException("Invalid Data Collector Guid.");
            }
            if (count < 0)
            {
                throw new ArgumentException("Invalid Count.");
            }
            try
            {
                string tmp = media + Environment.NewLine + settings.Replace(Environment.NewLine, "") + 
                                    Environment.NewLine + BitConverter.ToString(data).Replace("-", "");
                GXTaskUpdateRequest req = new GXTaskUpdateRequest(new GXAmiTask[] { new GXAmiTask(TaskType.MediaWrite, collectorGuid, tmp) });
                GXTaskUpdateResponse res = Client.Put(req);
                return res.Tasks[0];
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Start monitoring.
        /// </summary>
        /// <param name="target"></param>
        public GXAmiTask StartMonitoring(object target)
        {
            try
            {
                GXTaskUpdateRequest req = new GXTaskUpdateRequest(new GXAmiTask[] { new GXAmiTask(TaskType.StartMonitor, target) });
                GXTaskUpdateResponse res = Client.Put(req);
                return res.Tasks[0];
            } 
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Stop monitoring.
        /// </summary>
        /// <param name="target"></param>
        public GXAmiTask StopMonitoring(object target)
        {
            try
            {
                GXTaskUpdateRequest req = new GXTaskUpdateRequest(new GXAmiTask[] { new GXAmiTask(TaskType.StopMonitor, target) });
                GXTaskUpdateResponse res = Client.Put(req);
                return res.Tasks[0];
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Run Schedule.
        /// </summary>
        /// <param name="schedules">Schedule to run.</param>
        public GXAmiTask[] RunSchedule(GXAmiSchedule[] schedules)
        {
            try
            {
                List<GXAmiTask> tasks = new List<GXAmiTask>();
                foreach (GXAmiSchedule it in schedules)
                {
                    tasks.Add(new GXAmiTask(TaskType.RunSchedule, it));
                }
                GXTaskUpdateRequest req = new GXTaskUpdateRequest(tasks.ToArray());
                GXTaskUpdateResponse res = Client.Put(req);
                return res.Tasks;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Start Schedule.
        /// </summary>
        /// <param name="schedules">Schedule to start.</param>
        public GXAmiTask[] StartSchedules(GXAmiSchedule[] schedules)
        {
            try
            {
                List<GXAmiTask> tasks = new List<GXAmiTask>();
                foreach (GXAmiSchedule it in schedules)
                {
                    tasks.Add(new GXAmiTask(TaskType.StartSchedule, it));
                }
                GXTaskUpdateRequest req = new GXTaskUpdateRequest(tasks.ToArray());
                GXTaskUpdateResponse res = Client.Put(req);
                return res.Tasks;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Stop Schedule.
        /// </summary>
        /// <param name="schedules">Schedule to Stop.</param>
        public GXAmiTask[] StopSchedule(GXAmiSchedule[] schedules)
        {
            try
            {
                List<GXAmiTask> tasks = new List<GXAmiTask>();
                foreach (GXAmiSchedule it in schedules)
                {
                    tasks.Add(new GXAmiTask(TaskType.StopSchedule, it));
                }
                GXTaskUpdateRequest req = new GXTaskUpdateRequest(tasks.ToArray());
                GXTaskUpdateResponse res = Client.Put(req);
                return res.Tasks;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Add new device template to the users.
        /// </summary>
        /// <param name="groups">User groups that can see the device template.</param>
        /// <param name="device">GXAmiDevice to add.</param>
        public void AddDeviceTemplate(GXAmiUserGroup[] groups, GXDevice device)
        {
            if (device == null)
            {
                throw new ArgumentNullException();
            }
            try
            {
                string name = Path.GetTempFileName();
                GXZip.Export(device, name);
                byte[] data;
                using (Stream stream = File.OpenRead(name))
                {
                    BinaryReader r = new BinaryReader(stream);
                    data = r.ReadBytes((int)stream.Length);
                }
                GXDeviceTemplateUpdateRequest req = new GXDeviceTemplateUpdateRequest(groups, data);
                GXDeviceTemplateUpdateResponse res = Client.Post(req);
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
            }
        }        

        /// <summary>
        /// Remove device template.
        /// </summary>
        /// <param name="templates"></param>
        /// <param name="permamently">Is item removed permamently.</param>        
        public void RemoveDeviceTemplates(GXAmiDeviceTemplate[] templates, bool permamently)
        {
            try
            {
                GXDeviceTemplateDeleteRequest req = new GXDeviceTemplateDeleteRequest(templates, permamently);
                GXDeviceTemplateDeleteResponse res = Client.Delete(req);
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
            }
        }

        /// <summary>
        /// Is database and tables created.
        /// </summary>
        public bool IsDatabaseCreated()
        {
            try
            {
                //Client might be null when app is closing.
                if (Client != null)
                {
                    GXIsDatabaseCreatedRequest req = new GXIsDatabaseCreatedRequest();
                    GXIsDatabaseCreatedResponse ret = Client.Post(req);
                    return ret.IsCreate;
                }
                return false;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return false;
            }
        }

        /// <summary>
        /// Update item.
        /// </summary>
        /// <param name="target"></param>
        public void Update(object target)
        {
            //Client might be null when app is closing.
            if (Client == null)
            {
                return;
            }
            try
            {
                if (target is GXAmiUser)
                {
                    if ((target as GXAmiUser).Id == 0)
                    {
                        throw new ArgumentException("Invalid Id");
                    }
                    GXUserUpdateRequest req = new GXUserUpdateRequest(Actions.Edit, new GXAmiUser[] { target as GXAmiUser });
                    GXUserUpdateResponse res = Client.Put(req);
                    return;
                }
                if (target is GXAmiUserGroup)
                {
                    if ((target as GXAmiUserGroup).Id == 0)
                    {
                        throw new ArgumentException("Invalid Id");
                    }
                    GXUserGroupUpdateRequest req = new GXUserGroupUpdateRequest(Actions.Edit, new GXAmiUserGroup[] { target as GXAmiUserGroup });
                    GXUserGroupUpdateResponse res = Client.Put(req);
                    return;
                }
                if (target is GXAmiDevice)
                {
                    if ((target as GXAmiDevice).Id == 0)
                    {
                        throw new ArgumentException("Invalid Id");
                    }
                    GXDeviceUpdateRequest req = new GXDeviceUpdateRequest(Actions.Edit, new GXAmiDevice[] { target as GXAmiDevice });
                    GXDeviceUpdateResponse res = Client.Put(req);
                    return;
                }
                if (target is GXAmiDeviceGroup)
                {
                    if ((target as GXAmiDeviceGroup).Id == 0)
                    {
                        throw new ArgumentException("Invalid Id");
                    }
                    GXDeviceGroupUpdateRequest req = new GXDeviceGroupUpdateRequest(Actions.Edit, new GXAmiDeviceGroup[] { target as GXAmiDeviceGroup });
                    GXDeviceGroupUpdateResponse res = Client.Put(req);
                    return;
                }
                if (target is GXAmiTask)
                {
                    if ((target as GXAmiTask).Id == 0)
                    {
                        throw new ArgumentException("Invalid Id");
                    }
                    GXTaskUpdateRequest req = new GXTaskUpdateRequest(new GXAmiTask[] { target as GXAmiTask });
                    GXTaskUpdateResponse res = Client.Put(req);
                    return;
                }
                if (target is GXAmiSchedule)
                {
                    if ((target as GXAmiSchedule).Id == 0)
                    {
                        throw new ArgumentException("Invalid Id");
                    }
                    GXScheduleUpdateRequest req = new GXScheduleUpdateRequest(new GXAmiSchedule[] { target as GXAmiSchedule });
                    GXScheduleUpdateResponse res = Client.Put(req);
                    return;
                }
                if (target is GXAmiDataCollector)
                {
                    if ((target as GXAmiDataCollector).Id == 0)
                    {
                        throw new ArgumentException("Invalid Id");
                    }
                    GXDataCollectorUpdateRequest req = new GXDataCollectorUpdateRequest(new GXAmiDataCollector[] { target as GXAmiDataCollector }, (GXAmiDevice[])null);
                    GXDataCollectorUpdateResponse res = Client.Put(req);
                    return;
                }
                if (target is GXAmiParameter[])
                {
                    GXParameterUpdateRequest req = new GXParameterUpdateRequest(target as GXAmiParameter[]);
                    Client.Post(req);
                    return;
                }
                

            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
            }
            throw new Exception("Invalid target");
        }

        /// <summary>
        /// Datacollector adds new device error.
        /// </summary>
        /// <param name="target">Target that cause exception.</param>
        /// <param name="ex">Occurred exception.</param>
        /// <param name="severity">Exception severity.</param>
        public void AddDeviceError(GXAmiTask task, Exception ex, int severity)
        {
            //Client might be null when app is closing.
            if (Client != null)
            {
                GXErrorUpdateRequest req = new GXErrorUpdateRequest(task.TargetDeviceID, task.Id, severity, ex);
                GXErrorUpdateResponse res = Client.Put(req);
            }
        }

        /// <summary>
        /// Returns latest values.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public GXAmiDataValue[] GetLatestValues(GXAmiDevice target)
        {
            try
            {
                //Client might be null when app is closing.
                if (Client != null)
                {
                    GXValuesRequest req = new GXValuesRequest(new GXAmiDevice[] { target }, false);
                    GXValuesResponse res = Client.Post(req);
                    return res.Values;
                }
                return null;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Returns latest values.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public GXAmiDataValue[] GetLogValues(GXAmiDevice target)
        {
            try
            {  
                //Client might be null when app is closing.
                if (Client != null)
                {
                    GXValuesRequest req = new GXValuesRequest(new GXAmiDevice[] { target }, true);
                    GXValuesResponse res = Client.Post(req);
                    return res.Values;
                }
                return null;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Update device value.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        public void UpdateValue(GXAmiProperty target, object value)
        {
            try
            {
                //Client might be null when app is closing.
                if (Client != null)
                {
                    GXValuesUpdateRequest req = new GXValuesUpdateRequest(target, value);
                    Client.Post(req);
                }
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
            }
        }     

        /// <summary>
        /// Update device values.
        /// </summary>
        /// <param name="values"></param>
        public void UpdateValues(GXAmiDataValue[] values)
        {
            try
            {
                //Client might be null when app is closing.
                if (Client != null)
                {
                    GXValuesUpdateRequest req = new GXValuesUpdateRequest(values);
                    Client.Post(req);
                }
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
            }
        }

        /// <summary>
        /// Returns row count of selected table.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public long GetTableRowCount(GXAmiDataTable table)
        {
            try
            {
                //Client might be null when app is closing.
                if (Client != null)
                {
                    GXTableRequest req = new GXTableRequest(table);
                    GXTableResponse res = Client.Post(req);
                    return res.Count;
                }
                return 0;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return 0;
            }            
        }

        /// <summary>
        /// Set new trace level for selected Device or Data Collector.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="level"></param>
        public void SetTraceLevel(object target, System.Diagnostics.TraceLevel level)
        {
            try
            {
                if (target is GXAmiDataCollector)
                {
                    GXTraceUpdateRequest req = new GXTraceUpdateRequest(new GXAmiDataCollector[]{target as GXAmiDataCollector}, level);
                    Client.Post(req);                    
                }
                else if (target is GXAmiDevice)
                {
                    GXTraceUpdateRequest req = new GXTraceUpdateRequest(new GXAmiDevice[] { target as GXAmiDevice }, level);
                    Client.Post(req);
                }
                else
                {
                    throw new Exception("Trace level set failed. Unknown target.");
                }
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);                
            }
        }

        /// <summary>
        /// Get trace level for selected Device or Data Collector.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="level"></param>
        public System.Diagnostics.TraceLevel GetTraceLevel(object target)
        {
            try
            {
                GXTraceLevelResponse ret;
                if (target is GXAmiDataCollector)
                {
                    GXTraceLevelRequest req = new GXTraceLevelRequest(new GXAmiDataCollector[] { target as GXAmiDataCollector });
                    ret = Client.Get(req);
                }
                else if (target is GXAmiDevice)
                {
                    GXTraceLevelRequest req = new GXTraceLevelRequest(new GXAmiDevice[] { target as GXAmiDevice });
                    ret = Client.Get(req);
                }
                else if (target is GXAmiClient)
                {
                    GXTraceLevelRequest req = new GXTraceLevelRequest((target as GXAmiClient).DataCollectorGuid);
                    ret = Client.Get(req);
                }
                else
                {
                    throw new Exception("Trace level set failed. Unknown target.");
                }
                return ret.Levels[0];
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return System.Diagnostics.TraceLevel.Off;
            }
        }

        /// <summary>
        /// Get item traces.
        /// </summary>
        /// <param name="target"></param>
        public GXAmiTrace[] GetTraces(object target)
        {
            try
            {
                GXTracesResponse ret;
                if (target is GXAmiDataCollector)
                {
                    GXTracesRequest req = new GXTracesRequest(new GXAmiDataCollector[] { target as GXAmiDataCollector });
                    ret = Client.Get(req);
                }
                else if (target is GXAmiDevice)
                {
                    GXTracesRequest req = new GXTracesRequest(new GXAmiDevice[] { target as GXAmiDevice });
                    ret = Client.Get(req);
                }
                else
                {
                    throw new Exception("Get traces failed. Unknown target.");
                }
                return ret.Traces;
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Set new trace level for selected Device or Data Collector.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="level"></param>
        internal void AddTraces(GXAmiTrace[] traces)
        {
            try
            {
                GXTraceAddRequest req = new GXTraceAddRequest(traces);
                Client.Post(req);
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
            }
        }

        /// <summary>
        /// Clear traces.
        /// </summary>
        public void ClearTraces(GXAmiTrace[] traces)
        {
            try
            {
                GXTraceDeleteRequest req = new GXTraceDeleteRequest(traces);
                Client.Delete(req);
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
            }
        }

        /// <summary>
        /// Clear traces.
        /// </summary>
        public void ClearTraces(GXAmiDevice[] devices)
        {
            try
            {
                GXTraceDeleteRequest req = new GXTraceDeleteRequest(devices);
                Client.Post(req);
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
            }
        }


        /// <summary>
        /// Clear traces.
        /// </summary>
        public void ClearTraces(GXAmiDataCollector[] collectors)
        {
            try
            {
                GXTraceDeleteRequest req = new GXTraceDeleteRequest(collectors);
                Client.Post(req);
            }
            catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
            {
                ThrowException(ex);
            }
        }

        /// <summary>
        /// Returns rows of selected table.
        /// </summary>
        /// <param name="table">Table where rows are read.</param>
        /// <param name="index">Start index.</param>
        /// <param name="count">Rows count. Set to 0 if all rows are read.</param>
        /// <returns></returns>
        public object[][] GetTableRows(GXAmiDataTable table, int index, int count)
        {
            //Client might be null when app is closing.
            if (Client != null)
            {
                if (index < 0 || count < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                try
                {
                    GXTableRequest req = new GXTableRequest(table, index, count);
                    GXTableResponse res = Client.Post(req);
                    return res.Rows.ToArray();
                }
                catch (ServiceStack.ServiceClient.Web.WebServiceException ex)
                {
                    ThrowException(ex);
                    return null;
                }
            }
            return null;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (Client != null)
            {
                StopListenEvents();
				Client.CancelAsync();
                Client.Dispose();
                Client = null;
            }
        }

        #endregion
    }
}