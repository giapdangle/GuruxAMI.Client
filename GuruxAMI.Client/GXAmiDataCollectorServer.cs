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
using Gurux.Device.PresetDevices;
using System.Xml;
using System.Runtime.Serialization;

namespace GuruxAMI.Client
{
    public class GXSerialPortInfo
    {
        /// <summary>
        /// Available serial ports.
        /// </summary>
        public string[] SerialPorts;
    }

    /// <summary>
    /// Returs info from available serial ports.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void AvailableSerialPortsEventHandler(object sender, GXSerialPortInfo e);

    /// <summary>
    /// This class implements data collector.
    /// </summary>    
    public class GXAmiDataCollectorServer : IDisposable
    {
        System.Diagnostics.TraceLevel TraceLevel = System.Diagnostics.TraceLevel.Off;
        TasksAddedEventHandler m_TasksAdded;
        TasksClaimedEventHandler m_TasksClaimed;
        TasksRemovedEventHandler m_TasksRemoved;
        AvailableSerialPortsEventHandler m_AvailableSerialPorts;
        internal Gurux.Common.ErrorEventHandler m_Error;        
        
        /// <summary>
        /// New task is added or claimed.
        /// </summary>
        AutoResetEvent TaskModified = new AutoResetEvent(false);

        ManualResetEvent Closing = new ManualResetEvent(false);
        Thread Thread;

        GXAmiClient DC;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="guid">Data Collector Guid.</param>
        public GXAmiDataCollectorServer(string baseUr, Guid guid)
        {
            DC = new GXAmiClient(baseUr, guid);
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
        /// Device error(s) are added.
        /// </summary>
        /// <param name="sender"></param>
        public event Gurux.Common.ErrorEventHandler OnError
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

        /// <summary>
        /// Return available serial ports.
        /// </summary>
        /// <param name="sender"></param>
        public event AvailableSerialPortsEventHandler OnAvailableSerialPorts
        {
            add
            {
                m_AvailableSerialPorts += value;
            }
            remove
            {
                m_AvailableSerialPorts -= value;
            }
        }

        public Guid Guid
        {
            get
            {                
                if (DC == null)
                {
                    return Guid.Empty;
                }
                return DC.DataCollectorGuid;
            }
        }

        /// <summary>
        /// Initialize data collector. Null is returned if collector is already registered.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GXAmiDataCollector Init(string name)
        {            
            GXAmiDataCollector dc = null;
            if (DC.DataCollectorGuid == Guid.Empty)
            {
                dc = DC.RegisterDataCollector();
                dc.Name = name;
                dc.Medias = Gurux.Communication.GXClient.GetAvailableMedias();
                if (m_AvailableSerialPorts != null)
                {
                    GXSerialPortInfo e = new GXSerialPortInfo();
                    m_AvailableSerialPorts(this, e);
                    dc.SerialPorts = e.SerialPorts;
                }
                TraceLevel = dc.TraceLevel;
                DC.Update(dc);
            }
            else
            {                
                dc = DC.GetDataCollectorByGuid(DC.DataCollectorGuid);
                string[] old = dc.Medias;
                dc.Medias = Gurux.Communication.GXClient.GetAvailableMedias();
                bool changed = old != dc.Medias;
                if (m_AvailableSerialPorts != null)
                {
                    GXSerialPortInfo e = new GXSerialPortInfo();
                    m_AvailableSerialPorts(this, e);
                    old = dc.SerialPorts;
                    dc.SerialPorts = e.SerialPorts;
                    changed |= old != dc.SerialPorts;                    
                }
                TraceLevel = DC.GetTraceLevel(DC);
                //If medias or serial ports are changed.
                if (changed)
                {
                    DC.Update(dc);
                }
            }
            DC.OnTasksClaimed += new TasksClaimedEventHandler(DC_OnTasksClaimed);
            DC.OnTasksAdded += new TasksAddedEventHandler(DC_OnTasksAdded);
            DC.OnTasksRemoved += new TasksRemovedEventHandler(DC_OnTasksRemoved);
            DC.OnDevicesUpdated += new DevicesUpdatedEventHandler(DC_OnDevicesUpdated);
            DC.OnDevicesRemoved += new DevicesRemovedEventHandler(DC_OnDevicesRemoved);
            DC.OnDataCollectorsUpdated += new DataCollectorsUpdatedEventHandler(DC_OnDataCollectorsUpdated);
            DC.StartListenEvents();
            //Start Data Collector thread.
            Thread = new Thread(new ThreadStart(Run));
            Thread.IsBackground = true;
            Thread.Start();
            return dc;
        }

        void DC_OnDataCollectorsUpdated(object sender, GXAmiDataCollector[] collectors)
        {
            foreach (GXAmiDataCollector it in collectors)
            {
                if (it.Guid == this.DC.DataCollectorGuid)
                {
                    TraceLevel = it.TraceLevel;
                    break;
                }
            }
        }

        /// <summary>
        /// Update data collector settings.
        /// </summary>
        /// <param name="dc"></param>
        public void Update(GXAmiDataCollector dc)
        {
            DC.Update(dc);
        }

        /// <summary>
        /// Close DC and stop listen events.
        /// </summary>
        public void Close()
        {
            Closing.Set();
            DC.OnTasksClaimed -= new TasksClaimedEventHandler(DC_OnTasksClaimed);
            DC.OnTasksAdded -= new TasksAddedEventHandler(DC_OnTasksAdded);
            DC.OnTasksRemoved -= new TasksRemovedEventHandler(DC_OnTasksRemoved);
            DC.OnDevicesUpdated -= new DevicesUpdatedEventHandler(DC_OnDevicesUpdated);
            DC.OnDevicesRemoved -= new DevicesRemovedEventHandler(DC_OnDevicesRemoved);
            DC.OnDataCollectorsUpdated -= new DataCollectorsUpdatedEventHandler(DC_OnDataCollectorsUpdated);
            DC.StopListenEvents();
        }

        /// <summary>
        /// If device is removed remove device templates.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="devices"></param>
        void DC_OnDevicesRemoved(object sender, GXAmiDevice[] devices)
        {
            //TODO: Delete template files.
        }

        /// <summary>
        /// Device settings are changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="devices"></param>
        void DC_OnDevicesUpdated(object sender, GXAmiDevice[] devices)
        {
            return;
        }        

        /// <summary>
        /// Reserved for inner use.
        /// </summary>
        class GXClaimedInfo
        {
            /// <summary>
            /// Work thread 
            /// </summary>
            public Thread WorkThread;

            /// <summary>
            /// Collector is executed the task.
            /// </summary>
            public AutoResetEvent TaskExecuted = new AutoResetEvent(false);
            
            /// <summary>
            /// Set when new task is arrived.
            /// </summary>
            public AutoResetEvent NewTask = new AutoResetEvent(true);

            /// <summary>
            /// Set when thread can close work.
            /// </summary>
            public ManualResetEvent Closing = new ManualResetEvent(false);
            
            /// <summary>
            /// List of handled tasks.
            /// </summary>
            public List<GXClaimedTask> ClaimedTasks = new List<GXClaimedTask>();
            
            /// <summary>
            /// List of occurred exceptions.
            /// </summary>
            public Dictionary<GXClaimedTask, Exception> Exceptions = new Dictionary<GXClaimedTask, Exception>(); 

        }

        /// <summary>
        /// Save executed tasks so they can be retreaved if app is restarted.
        /// </summary>
        /// <remarks>
        /// Example monitoring uses this.
        /// </remarks>
        static void SaveExecutedTask(Guid guid, GXClaimedTask task)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;
            settings.CloseOutput = true;
            settings.CheckCharacters = false;
            System.Runtime.Serialization.DataContractSerializer x = new System.Runtime.Serialization.DataContractSerializer(typeof(GXClaimedTask));
            using (XmlWriter writer = XmlWriter.Create(guid.ToString() + "executingTask.xml", settings))
            {
                x.WriteObject(writer, task);
            }
        }

        /// <summary>
        /// Load executed tasks so they can be retreaved if app is restarted.
        /// </summary>
        /// <remarks>
        /// Example monitoring uses this.
        /// </remarks>
        static GXClaimedTask LoadExecutedTask(Guid guid)
        {
            try
            {
                string path = guid.ToString() + "executingTask.xml";
                if (File.Exists(path))
                {
                    System.Runtime.Serialization.DataContractSerializer x = new System.Runtime.Serialization.DataContractSerializer(typeof(GXClaimedTask), new Type[] { typeof(GXAmiTask[]) });
                    using (FileStream reader = new FileStream(path, FileMode.Open))
                    {
                        return x.ReadObject(reader) as GXClaimedTask;
                    }                 
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);                
            }
            return null;
        }

        /// <summary>
        /// Device collector thread.
        /// </summary>
        /// <param name="target"></param>
        void DeviceCollector(object target)
        {            
            GXClaimedInfo info = target as GXClaimedInfo;            
            string pathToDll = this.GetType().Assembly.CodeBase;
            // Create an Application Domain:
            AppDomainSetup domainSetup = new AppDomainSetup { PrivateBinPath = pathToDll };
            string dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + Path.DirectorySeparatorChar);
            Dictionary<string, IGXMedia> medias = new Dictionary<string,IGXMedia>();
            Guid loadedDeviceProfiles = Guid.Empty;
            System.AppDomain td = AppDomain.CreateDomain("DC_Domain", null, domainSetup);
            GXProxyClass pc = (GXProxyClass)(td.CreateInstanceFromAndUnwrap(pathToDll, typeof(GXProxyClass).FullName));
            GXAmiEventListener listener = new GXAmiEventListener(this, pc, DC);            
            string path = null;
            List<GXAmiTask> executedTasks = new List<GXAmiTask>();
            //Connection is leave open if meter is monitored.
            bool leaveConnectionOpen = false;
            GXClaimedTask taskinfo = null;            
            while (!info.Closing.WaitOne(1))
            {
                try
                {
                    while (!info.Closing.WaitOne(1))
                    {
                        bool moreTasks = false;
                        //Check is there more tasks to execute otherwice close the connection.
                        lock (info.ClaimedTasks)
                        {
                            foreach(var it in info.ClaimedTasks)
                            {
                                if (it.Task.State == TaskState.Pending)
                                {
                                    moreTasks = true;
                                }
                            }
                            //Close connection if meter is not monitored.
                            if (!moreTasks && !leaveConnectionOpen)
                            {
                                try
                                {
                                    pc.Disconnect();
                                }
                                catch (Exception ex)
                                {
                                    lock (info.Exceptions)
                                    {
                                        //Taskinfo is null in disconnecting.
                                        if (taskinfo != null)
                                        {
                                            info.Exceptions.Add(taskinfo, ex);
                                            taskinfo.Task.State = TaskState.Failed;
                                            info.TaskExecuted.Set();
                                        }
                                    }
                                    if (m_Error != null)
                                    {
                                        m_Error(this, ex);
                                    }
                                }
                            }
                        }
                        if (!moreTasks)
                        {
                            //Wait until next task received or app is closed.
                            if (EventWaitHandle.WaitAny(new EventWaitHandle[] { info.NewTask, info.Closing }) == 1)
                            {
                                break;
                            }
                        }
                        taskinfo = null;
                        System.Diagnostics.Debug.WriteLine("Checking task: " + Guid.ToString());
                        lock (info.ClaimedTasks)
                        {
                            System.Diagnostics.Debug.WriteLine("Checking task2 : " + Guid.ToString());
                            foreach (GXClaimedTask it in info.ClaimedTasks)
                            {                                
                                if (it.Task.State == TaskState.Pending)
                                {
                                    it.Task.State = TaskState.Processing;
                                    taskinfo = it;
                                    System.Diagnostics.Debug.WriteLine("Processing task: " + it.Task.Id + " " + Guid.ToString());
                                    break;
                                }
                            }                            
                        }

                        //If task is claimed.
                        if (taskinfo != null)
                        {
                            info.TaskExecuted.Reset();
                            if (taskinfo.Task.TaskType == TaskType.MediaGetProperty)
                            {
                                IGXMedia media = null;
                                if (medias.ContainsKey(taskinfo.MediaSettings[0].Value.Key))
                                {
                                    media = medias[taskinfo.MediaSettings[0].Value.Key];
                                }
                                else
                                {
                                    media = new Gurux.Communication.GXClient().SelectMedia(taskinfo.MediaSettings[0].Key);
                                    medias.Add(taskinfo.MediaSettings[0].Value.Key, media);
                                }
                                media.Tag = taskinfo.Task;
                                Dictionary<string, object> properties = new Dictionary<string, object>();
                                foreach (string it in taskinfo.Data.Split(';'))
                                {
                                    PropertyDescriptor prop = TypeDescriptor.GetProperties(media)[it];
                                    object value = prop.GetValue(media);
                                    taskinfo.Task.State = TaskState.Succeeded;
                                    properties.Add(it, value);
                                }
                                DC.SetMediaProperties(this.Guid, taskinfo.MediaSettings[0].Key, taskinfo.MediaSettings[0].Value.Key, properties, taskinfo.Task.Id);                                
                            }
                            else if (taskinfo.Task.TaskType == TaskType.MediaSetProperty)
                            {
                                IGXMedia media = null;
                                if (medias.ContainsKey(taskinfo.MediaSettings[0].Value.Key))
                                {
                                    media = medias[taskinfo.MediaSettings[0].Value.Key];
                                }
                                else
                                {
                                    media = new Gurux.Communication.GXClient().SelectMedia(taskinfo.MediaSettings[0].Key);
                                    medias.Add(taskinfo.MediaSettings[0].Value.Key, media);
                                }
                                media.Tag = taskinfo.Task;
                                string[] tmp = taskinfo.Data.Split(new string[] {Environment.NewLine}, StringSplitOptions.None);
                                string[] names = tmp[0].Split(new char[] { ';' });
                                string[] values = tmp[1].Split(new char[] { ';' });
                                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(media);
                                for (int pos = 0; pos != names.Length; ++pos)
                                {
                                    PropertyDescriptor prop = properties[names[pos]];
                                    object value;
                                    if (prop.PropertyType.IsEnum)
                                    {
                                        value = Enum.Parse(prop.PropertyType, values[pos]);
                                    }
                                    else
                                    {
                                        value = Convert.ChangeType(values[pos], prop.PropertyType);
                                    }
                                    prop.SetValue(media, value);
                                }
                                taskinfo.Task.State = TaskState.Succeeded;                                
                            }
                            else if (taskinfo.Task.TaskType == TaskType.MediaOpen)
                            {
                                IGXMedia media = null;
                                leaveConnectionOpen = true;
                                if (medias.ContainsKey(taskinfo.MediaSettings[0].Value.Key))
                                {
                                    media = medias[taskinfo.MediaSettings[0].Value.Key];
                                    media.Tag = taskinfo.Task;
                                    try
                                    {
                                        if (media.IsOpen)
                                        {
                                            media_OnMediaStateChange(media, new MediaStateEventArgs(MediaState.Open));
                                        }
                                        else
                                        {
                                            if (TraceLevel != System.Diagnostics.TraceLevel.Off)
                                            {
                                                media.Trace = TraceLevel;
                                                media.OnTrace += new TraceEventHandler(media_OnTrace);
                                            }
                                            media.Settings = taskinfo.MediaSettings[0].Value.Value;
                                            media.OnReceived += new ReceivedEventHandler(media_OnReceived);
                                            media.OnMediaStateChange += new MediaStateChangeEventHandler(media_OnMediaStateChange);
                                            media.OnError += new Gurux.Common.ErrorEventHandler(media_OnError);
                                            media.Open();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        if (media != null)
                                        {
                                            if (TraceLevel != System.Diagnostics.TraceLevel.Off)
                                            {
                                                media.OnTrace -= new TraceEventHandler(media_OnTrace);
                                            }
                                            media.OnReceived -= new ReceivedEventHandler(media_OnReceived);
                                            media.OnMediaStateChange -= new MediaStateChangeEventHandler(media_OnMediaStateChange);
                                            media.OnError -= new Gurux.Common.ErrorEventHandler(media_OnError);
                                        }
                                        throw ex;
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        media = new Gurux.Communication.GXClient().SelectMedia(taskinfo.MediaSettings[0].Key);
                                        media.Tag = taskinfo.Task;
                                        if (TraceLevel != System.Diagnostics.TraceLevel.Off)
                                        {
                                            media.Trace = TraceLevel;
                                            media.OnTrace += new TraceEventHandler(media_OnTrace);
                                        }
                                        media.Settings = taskinfo.MediaSettings[0].Value.Value;
                                        media.OnReceived += new ReceivedEventHandler(media_OnReceived);
                                        media.OnMediaStateChange += new MediaStateChangeEventHandler(media_OnMediaStateChange);
                                        media.OnError += new Gurux.Common.ErrorEventHandler(media_OnError);
                                        media.Open();
                                        medias.Add(taskinfo.MediaSettings[0].Value.Key, media);
                                    }
                                    catch (Exception ex)
                                    {
                                        if (media != null)
                                        {
                                            if (TraceLevel != System.Diagnostics.TraceLevel.Off)
                                            {
                                                media.OnTrace -= new TraceEventHandler(media_OnTrace);
                                            }
                                            media.OnReceived -= new ReceivedEventHandler(media_OnReceived);
                                            media.OnMediaStateChange -= new MediaStateChangeEventHandler(media_OnMediaStateChange);
                                            media.OnError -= new Gurux.Common.ErrorEventHandler(media_OnError);
                                        }
                                        throw ex;
                                    }
                                }
                                taskinfo.Task.State = TaskState.Succeeded;                                
                            }
                            else if (taskinfo.Task.TaskType == TaskType.MediaClose)
                            {
                                if (medias.ContainsKey(taskinfo.MediaSettings[0].Value.Key))
                                {
                                    IGXMedia media = medias[taskinfo.MediaSettings[0].Value.Key];
                                    media.Tag = taskinfo.Task;
                                    medias.Remove(taskinfo.MediaSettings[0].Value.Key);
                                    media.Close();
                                    media.OnMediaStateChange -= new MediaStateChangeEventHandler(media_OnMediaStateChange);
                                    media.OnError -= new Gurux.Common.ErrorEventHandler(media_OnError);
                                    media.OnReceived -= new ReceivedEventHandler(media_OnReceived);
                                    if (TraceLevel != System.Diagnostics.TraceLevel.Off)
                                    {                                     
                                        media.OnTrace -= new TraceEventHandler(media_OnTrace);
                                    }
                                }
                                leaveConnectionOpen = false;
                                taskinfo.Task.State = TaskState.Succeeded;                                
                            }
                            else if (taskinfo.Task.TaskType == TaskType.MediaWrite)
                            {
                                IGXMedia media;
                                if (!medias.ContainsKey(taskinfo.MediaSettings[0].Value.Key))
                                {                                    
                                    media = new Gurux.Communication.GXClient().SelectMedia(taskinfo.MediaSettings[0].Key);
                                    media.Tag = taskinfo.Task;
                                    medias.Add(taskinfo.MediaSettings[0].Value.Key, media);
                                    media.Settings = taskinfo.MediaSettings[0].Value.Value;
                                    media.OnReceived += new ReceivedEventHandler(media_OnReceived);
                                    media.OnMediaStateChange += new MediaStateChangeEventHandler(media_OnMediaStateChange);
                                    media.OnError += new Gurux.Common.ErrorEventHandler(media_OnError);
                                    media.Open();
                                }
                                else
                                {
                                    media = medias[taskinfo.MediaSettings[0].Value.Key];
                                    media.Tag = taskinfo.Task;
                                }
                                media.Send(Gurux.Common.GXCommon.HexToBytes(taskinfo.Data, false), null);
                                taskinfo.Task.State = TaskState.Succeeded;                                
                            }
                            else if (taskinfo.Task.TaskType == TaskType.MediaState ||
                                taskinfo.Task.TaskType == TaskType.MediaError)
                            {
                                //This might happend is DC is closed and there are event info left.
                                taskinfo.Task.State = TaskState.Succeeded;                                
                            }
                            else
                            {
                                HandleDevice(info, pc, ref path, ref leaveConnectionOpen, taskinfo);
                            }
                            info.TaskExecuted.Set();
                        }
                    }
                }
                catch (Exception ex)
                {
                    lock (info.Exceptions)
                    {
                        //Taskinfo is null in disconnecting.
                        if (taskinfo != null)
                        {
                            //Mikko task voidaan ajaa useampaan kertaan esim schedule jolloin taskinfo voi jo olla.
                            info.Exceptions.Add(taskinfo, ex);
                            taskinfo.Task.State = TaskState.Failed;
                            info.TaskExecuted.Set();
                        }
                    }
                    if (m_Error != null)
                    {
                        m_Error(this, ex);
                    }
                }
            }
            try
            {
                pc.Close();
            }
            catch (System.Runtime.Remoting.RemotingException)
            {

            }
            //Unload app domain.
            System.AppDomain.Unload(td);
        }

        private void HandleDevice(GXClaimedInfo info, GXProxyClass pc, ref string path, ref bool leaveConnectionOpen, GXClaimedTask taskinfo)
        {
            try
            {
                //If device template is not loaded yet.
                if (path == null)
                {
                    path = Path.Combine(Gurux.Common.GXCommon.ApplicationDataPath, "Gurux");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                        Gurux.Common.GXFileSystemSecurity.UpdateDirectorySecurity(path);
                    }
                    path = Path.Combine(path, "Gurux.DeviceSuite");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                        Gurux.Common.GXFileSystemSecurity.UpdateDirectorySecurity(path);
                    }
                    path = Path.Combine(path, "DeviceProfiles");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                        Gurux.Common.GXFileSystemSecurity.UpdateDirectorySecurity(path);
                    }
                    path = Path.Combine(path, taskinfo.DeviceProfile.ToString());
                    //Load Device template if not loaded yet.                                 
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                        Gurux.Common.GXFileSystemSecurity.UpdateDirectorySecurity(path);
                        byte[] data = DC.GetDeviceProfilesData(taskinfo.DeviceProfile);
                        GXDeviceProfile type = pc.Import(data, path);
                        if (!(type is GXPublishedDeviceProfile))
                        {
                            File.Copy(type.Path, Path.Combine(path, taskinfo.DeviceProfile.ToString() + ".gxp"), true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                path = null;
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
                lock (info.Exceptions)
                {
                    info.Exceptions.Add(taskinfo, ex);
                    taskinfo.Task.State = TaskState.Failed;
                }
            }
            try
            {
                //Save executed task. This is used if error occures.                                    
                pc.Connect(path, taskinfo);
                //Read or write device.
                if (taskinfo.Task.TaskType == TaskType.Read ||
                    taskinfo.Task.TaskType == TaskType.Write)
                {
                    System.Diagnostics.Debug.WriteLine("DC start to read target: " + taskinfo.Task.Id.ToString() + " " + taskinfo.Task.TargetDeviceID.ToString());
                    pc.ExecuteTransaction(taskinfo);
                    taskinfo.Task.State = TaskState.Succeeded;
                    System.Diagnostics.Debug.WriteLine("DC end reading target: " + taskinfo.Task.Id.ToString() + " " + Guid.ToString());                                        
                }
                else if (taskinfo.Task.TaskType == TaskType.StartMonitor)
                {
                    leaveConnectionOpen = true;
                    pc.StartMonitoring();
                    taskinfo.Task.State = TaskState.Succeeded;
                }
                else if (taskinfo.Task.TaskType == TaskType.StopMonitor)
                {
                    leaveConnectionOpen = false;
                    pc.StopMonitoring();
                    taskinfo.Task.State = TaskState.Succeeded;
                }
                else
                {
                    throw new Exception("Invalid task type.");
                }
            }
            catch (Exception ex)
            {
                lock (info.Exceptions)
                {
                    info.Exceptions.Add(taskinfo, ex);
                    taskinfo.Task.State = TaskState.Failed;
                }
                if (m_Error != null)
                {
                    m_Error(this, ex);
                }
            }
        }

        void media_OnTrace(object sender, TraceEventArgs e)
        {
            if (DC != null)
            {
                GXAmiTrace trace = new GXAmiTrace();
                trace.Type = e.Type;
                trace.Data = e.DataToString(false);
                trace.DataType = e.Data.GetType().FullName;
                DC.AddTraces(new GXAmiTrace[] { trace });
            }
        }

        /// <summary>
        /// Report Media error.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ex"></param>
        void media_OnError(object sender, Exception ex)
        {
            if (DC != null)
            {
                DC.MediaError((sender as IGXMedia).Tag as GXAmiTask, ex);
            }
        }

        /// <summary>
        /// Report Media state change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void media_OnMediaStateChange(object sender, MediaStateEventArgs e)
        {
            if (DC != null && (e.State == MediaState.Open || e.State == MediaState.Closing))
            {
                IGXMedia media = sender as IGXMedia;
                GXAmiTask t = media.Tag as GXAmiTask;
                ulong replyId = 0;
                if (t != null)
                {
                    replyId = t.Id;
                }
                DC.MediaStateChange(DC.DataCollectorGuid, media.MediaType, media.Name, e.State, replyId);
            }
        }

        void media_OnReceived(object sender, ReceiveEventArgs e)
        {
            IGXMedia media = sender as IGXMedia;
            DC.MediaWrite(DC.DataCollectorGuid, media.MediaType, media.Name, (byte[])e.Data, null);
        }
        
        /// <summary>
        /// Tasks for the Devices
        /// </summary>
        List<GXAmiTask> UnclaimedTasks = new List<GXAmiTask>();

        /// <summary>
        /// List of device IDs and executed tasks.
        /// </summary>        
        Dictionary<object, GXClaimedInfo> ClaimedTasks = new Dictionary<object, GXClaimedInfo>();

        /// <summary>
        /// Wait new tasks and and create instance from DC if not created yet.
        /// </summary>
        void Run()
        {            
            //Read tasks that was prosessed when app was closed.
            //This is used to start example monitoring again.
            GXClaimedTask executedTask = LoadExecutedTask(this.Guid);
            if (executedTask != null)
            {
                if (executedTask.Task.TaskType == TaskType.StartMonitor)
                {
                    TaskModified.Set();
                }
            }
            else
            {
                GXAmiTask[] pendingTasks = DC.GetTasks(TaskState.Pending, false);
                if (pendingTasks.Length != 0)
                {
                    DC_OnTasksAdded(DC, pendingTasks);
                }
            }
            //Save device threads so we can wait whem on close.
            while (true)
            {
                List<EventWaitHandle> events = new List<EventWaitHandle>();
                events.Add(Closing);
                events.Add(TaskModified);
                foreach (GXClaimedInfo it in ClaimedTasks.Values.ToArray())
                {
                    events.Add(it.TaskExecuted);
                }

                int ret = EventWaitHandle.WaitAny(events.ToArray());
                //If closing.
                if (ret == 0)
                {
                    break;
                }                
                //If new task added.
                if (ret == 1)
                {
                    System.Diagnostics.Debug.WriteLine("DC task added: " + Guid.ToString());
                    //Get new task
                    GXClaimedTask taskinfo = executedTask;
                    executedTask = null;
                    lock (UnclaimedTasks)
                    {
                        for (int pos = 0; pos != UnclaimedTasks.Count; ++pos)
                        {
                                GXAmiTask task = UnclaimedTasks[pos];
                                //New task added.
                                if (task.State == TaskState.Pending)
                                {
                                    lock (UnclaimedTasks)
                                    {
                                        bool idle = true;
                                        //If task is sent to the device
                                        if (task.TargetDeviceID != null)
                                        {
                                            idle = !ClaimedTasks.ContainsKey(task.TargetDeviceID.Value) ||
                                                ClaimedTasks[task.TargetDeviceID.Value].ClaimedTasks.Count == 0;
                                        }
                                        else //If task is sent to the DC.
                                        {
                                            idle = !ClaimedTasks.ContainsKey(task.DataCollectorGuid) ||
                                                ClaimedTasks[task.DataCollectorGuid].ClaimedTasks.Count == 0;
                                        }
                                        //If collector is reading do not claim task before collector is finished reading.
                                        if (idle)
                                        {
                                            try
                                            {
                                                taskinfo = DC.ClaimTask(task);
                                                //If other DC is claimed the task.
                                                if (taskinfo == null || task.Id != taskinfo.Task.Id)
                                                {
                                                    //Remove task that was asked, but failed.
                                                    UnclaimedTasks.Remove(task);
                                                    //Remove executed task.
                                                    if (taskinfo != null && task.Id != taskinfo.Task.Id)
                                                    {
                                                        UnclaimedTasks.Remove(taskinfo.Task);
                                                        if (taskinfo.Task.State == TaskState.Processing)
                                                        {
                                                            System.Diagnostics.Debug.Assert(false);
                                                        }
                                                        task = taskinfo.Task;
                                                    }
                                                }
                                                else //DC claimed task.
                                                {
                                                    task = taskinfo.Task;
                                                    UnclaimedTasks.Remove(task);
                                                }
                                                if (taskinfo != null)
                                                {
                                                    //This should be newer happend.
                                                    if (task.State == TaskState.Processing)
                                                    {
                                                        System.Diagnostics.Debug.Assert(false);
                                                    }
                                                    System.Diagnostics.Debug.WriteLine("DC Start to process task: " + Guid.ToString() + " " + task.Id.ToString() + " " + task.TargetDeviceID.ToString());
                                                    if (m_TasksClaimed != null)
                                                    {
                                                        m_TasksClaimed(this, new GXAmiTask[] { task });
                                                    }
                                                }
                                                else
                                                {
                                                    System.Diagnostics.Debug.WriteLine("DC Failed to get task: " + Guid.ToString() + " " + task.Id.ToString());
                                                }
                                                break;
                                            }
                                            catch (Exception ex)
                                            {
                                                if (m_Error != null)
                                                {
                                                    m_Error(this, ex);
                                                }
                                            }
                                        }
                                        else//If task is processed. Wait until task is executed and read again...
                                        {
                                            System.Diagnostics.Debug.WriteLine("DC is already processing task: " + Guid.ToString());
                                            if (task.TargetDeviceID != null)
                                            {
                                                ClaimedTasks[task.TargetDeviceID].TaskExecuted.WaitOne(50000);
                                            }
                                            else
                                            {
                                                ClaimedTasks[task.DataCollectorGuid].TaskExecuted.WaitOne(50000);
                                            }
                                            break;
                                        }
                                    }
                                }
                        }
                    }
                    //If new task is claimed.
                    if (taskinfo != null)
                    {
                        if (taskinfo.Task.TargetDeviceID != null)
                        {
                            SaveExecutedTask(this.Guid, taskinfo);
                            GXClaimedInfo dcinfo = null;
                            //If task is sent to existing device.
                            if (ClaimedTasks.ContainsKey(taskinfo.Task.TargetDeviceID.Value))
                            {
                                dcinfo = ClaimedTasks[taskinfo.Task.TargetDeviceID.Value];
                                lock (dcinfo.ClaimedTasks)
                                {
                                    dcinfo.ClaimedTasks.Add(taskinfo);
                                    dcinfo.NewTask.Set();
                                }
                            }
                            else//If task is sent to new device.
                            {
                                dcinfo = new GXClaimedInfo();
                                dcinfo.ClaimedTasks.Add(taskinfo);                                
                                ClaimedTasks.Add(taskinfo.Task.TargetDeviceID.Value, dcinfo);
                                Thread thread = new Thread(new ParameterizedThreadStart(DeviceCollector));
                                thread.Start(dcinfo);
                                dcinfo.WorkThread = thread;
                            }                            
                        }
                        //Task is send to Media
                        else if (taskinfo.Task.TargetType == TargetType.Media)
                        {
                            GXClaimedInfo dcinfo = null;
                            if (ClaimedTasks.ContainsKey(taskinfo.Task.DataCollectorGuid))
                            {
                                dcinfo = ClaimedTasks[taskinfo.Task.DataCollectorGuid];
                                lock (dcinfo.ClaimedTasks)
                                {
                                    dcinfo.ClaimedTasks.Add(taskinfo);
                                    dcinfo.NewTask.Set();
                                }
                            }
                            else
                            {
                                dcinfo = new GXClaimedInfo();
                                dcinfo.ClaimedTasks.Add(taskinfo);                                
                                ClaimedTasks.Add(taskinfo.Task.DataCollectorGuid, dcinfo);
                                Thread thread = new Thread(new ParameterizedThreadStart(DeviceCollector));
                                thread.Start(dcinfo);
                                dcinfo.WorkThread = thread;
                            }
                        }
                    }
                }
                else
                {
                    ret -= 2;
                    GXClaimedInfo c = ClaimedTasks[ClaimedTasks.Keys.ElementAt(ret)];                    
                    c.TaskExecuted.Reset();
                    System.Diagnostics.Debug.WriteLine("Task handled: " + c.ClaimedTasks[0].Task.Id.ToString());
                    foreach (GXClaimedTask it2 in c.ClaimedTasks)
                    {
                        GXAmiTask task = it2.Task;                        
                        //This should be newer happend.
                        if (task.State == TaskState.Processing)
                        {
                            System.Diagnostics.Debug.Assert(false);
                        }
                        //Remove task after successful reading.
                        if (task.State == TaskState.Succeeded)
                        {
                            lock (c.ClaimedTasks)
                            {
                                c.ClaimedTasks.Remove(it2);                                
                            }
                            try
                            {
                                DC.RemoveTask(task);
                                //Close thread if device task is executed and connection is not leave open. 
                                if ( task.TaskType != TaskType.StartMonitor)
                                {
                                    if (task.TargetDeviceID != null)
                                    {
                                        c.Closing.Set();
                                        ClaimedTasks.Remove(task.TargetDeviceID.Value);                                        
                                    }
                                    else if (task.TaskType == TaskType.MediaClose)//if DC task is executed.
                                    {
                                        c.Closing.Set();
                                        ClaimedTasks.Remove(task.DataCollectorGuid);                                        
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                if (m_Error != null)
                                {
                                    m_Error(this, ex);
                                }
                            }
                            lock (UnclaimedTasks)
                            {
                                //Search next unclaimed task and execute it.
                                foreach (GXAmiTask ut in UnclaimedTasks)
                                {
                                    if (ut.State == TaskState.Pending)
                                    {
                                        TaskModified.Set();
                                        break;
                                    }
                                }
                            }
                            //System.Diagnostics.Debug.WriteLine("Wait next task.");
                            break; //Break here because task is removed.                                    
                        }
                        else if (task.State == TaskState.Timeout)
                        {
                            try
                            {
                                lock (c.ClaimedTasks)
                                {
                                    c.ClaimedTasks.Remove(it2);
                                }
                                DC.AddDeviceError(task, new TimeoutException(), 1);
                                //Remove failed task.
                                DC.RemoveTask(task);
                            }
                            catch (Exception ex)
                            {
                                if (m_Error != null)
                                {
                                    m_Error(this, ex);
                                }
                            }
                            break; //Break here because task is removed.      
                        }
                        else if (task.State == TaskState.Failed)
                        {
                            try
                            {
                                lock (c.ClaimedTasks)
                                {
                                    c.ClaimedTasks.Remove(it2);
                                }
                                foreach (var it in c.Exceptions)
                                {
                                    DC.AddDeviceError(it.Key.Task, it.Value, 1);
                                    /*
                                    //If device is caused the error.
                                    if (it.Key.Task.TargetDeviceID.HasValue)
                                    {
                                        DC.AddDeviceError(it.Key.Task, it.Value, 1);
                                    }
                                    //If DC is caused the error.
                                    else if (it.Key.Task.DataCollectorID != 0)
                                    {
                                        DC.AddDeviceError(it.Key.Task, it.Value, 1);
                                    }
                                    else
                                    {
                                        throw new Exception("Unknown target.");
                                    }
                                     * */
                                }
                                c.Exceptions.Clear();
                                //Remove failed task.
                                DC.RemoveTask(task);
                            }
                            catch (Exception ex)
                            {
                                if (m_Error != null)
                                {
                                    m_Error(this, ex);
                                }
                            }
                            break; //Break here because task is removed.      
                        }
                    }
                }
            }
            //Tell all collector threads to close.
            List<Thread> threads = new List<Thread>();
            foreach(GXClaimedInfo it in ClaimedTasks.Values)
            {                
                it.Closing.Set();
                threads.Add(it.WorkThread);
            }
            //Wait until threads are closed.
            foreach (Thread it in threads)
            {
                it.Join();
            }            
        }

        /// <summary>
        /// Some other data collector is clamed task.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="tasks"></param>
        void DC_OnTasksClaimed(object sender, GXAmiTask[] tasks)
        {
            lock (UnclaimedTasks)
            {
                foreach (GXAmiTask task in tasks)
                {
                    foreach (GXAmiTask t in UnclaimedTasks)
                    {
                        if (t.Id == task.Id && t.State == TaskState.Pending)
                        {
                            t.State = TaskState.Succeeded;
                            TaskModified.Set();
                            break;
                        }
                    }
                }                
            }
        } 

        /// <summary>
        /// New task is added to the DC.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="tasks"></param>
        void DC_OnTasksAdded(object sender, GXAmiTask[] tasks)
        {            
            lock (UnclaimedTasks)
            {
                foreach (GXAmiTask task in tasks)
                {
                    System.Diagnostics.Debug.WriteLine("New task added: " + task.Id);
                    if (task.DataCollectorGuid != Guid.Empty && 
                        DC.DataCollectorGuid != task.DataCollectorGuid)
                    {
                        throw new Exception("Wrong Data Collector.");
                    }                    
                }
                UnclaimedTasks.AddRange(tasks);
                if (m_TasksAdded != null)
                {
                    m_TasksAdded(this, tasks.ToArray());
                }
                TaskModified.Set();
            }
        }

        /// <summary>
        /// Task is removed from the DC.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="tasks"></param>
        void DC_OnTasksRemoved(object sender, GXAmiTask[] tasks)
        {
            lock (UnclaimedTasks)
            {
                foreach (GXAmiTask task in tasks)
                {
                    if (task.DataCollectorGuid != this.Guid)
                    {
                        throw new Exception("Wrong Data Collector.");
                    }
                    foreach (GXAmiTask t in UnclaimedTasks)
                    {
                        if (t.Id == task.Id)
                        {
                            UnclaimedTasks.Remove(t);
                            break;
                        }
                    }                    
                }
                if (m_TasksRemoved != null)
                {
                    m_TasksRemoved(this, tasks);
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {            
            if (DC != null)
            {
                //Close collector thread.                
                Closing.Set();
                TaskModified.Set();                
                if (Thread != null)
                {
                    Thread.Join();
                    Thread = null;
                }
                DC.Dispose();
                DC = null;
            }
        }
        #endregion        
    }
}
