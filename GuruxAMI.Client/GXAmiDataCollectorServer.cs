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
        GXClaimedTask taskinfo = null;
        

        AutoResetEvent TaskModified = new AutoResetEvent(false);
        ManualResetEvent Closing = new ManualResetEvent(false);
        ManualResetEvent Closed = new ManualResetEvent(false);
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
                TraceLevel = DC.GetTraceLevel(DC);
            }
            DC.OnTasksClaimed += new TasksClaimedEventHandler(DC_OnTasksClaimed);
            DC.OnTasksAdded += new TasksAddedEventHandler(DC_OnTasksAdded);
            DC.OnTasksRemoved += new TasksRemovedEventHandler(DC_OnTasksRemoved);
            DC.OnDevicesUpdated += new DevicesUpdatedEventHandler(DC_OnDevicesUpdated);
            DC.OnDevicesRemoved += new DevicesRemovedEventHandler(DC_OnDevicesRemoved);
            DC.OnDataCollectorsUpdated += new DataCollectorsUpdatedEventHandler(DC_OnDataCollectorsUpdated);
            DC.StartListenEvents();
            //Start Data Collector.
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

        public void Close()
        {
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
            /// Collector is executing the task.
            /// </summary>
            public ManualResetEvent ExecutingTask = new ManualResetEvent(false);
            /// <summary>
            /// Set when new task is arived.
            /// </summary>
            public AutoResetEvent NewTask = new AutoResetEvent(true);
            /// <summary>
            /// List of received tasks.
            /// </summary>
            public List<GXClaimedTask> ClaimedTasks = new List<GXClaimedTask>();
            
            /// <summary>
            /// List of occurred exceptions.
            /// </summary>
            public Dictionary<GXClaimedTask, Exception> Exceptions = new Dictionary<GXClaimedTask, Exception>(); 

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
            Guid loadedDeviceTemplate = Guid.Empty;
            System.AppDomain td = AppDomain.CreateDomain("DC_Domain", null, domainSetup);
            GXProxyClass pc = (GXProxyClass)(td.CreateInstanceFromAndUnwrap(pathToDll, typeof(GXProxyClass).FullName));
            GXAmiEventListener listener = new GXAmiEventListener(this, pc, DC);            
            string path = null;
            bool leaveConnectionOpen = false;
            while (!Closing.WaitOne(1))
            {
                try
                {
                    while (!Closing.WaitOne(1))
                    {
                        //Check is there more tasks to execute otherwice close the connection.
                        lock (info.ClaimedTasks)
                        {
                            if (!leaveConnectionOpen)
                            {
                                bool allClaimed = true;
                                foreach(var it in info.ClaimedTasks)
                                {
                                    if (it.Task.State == TaskState.Pending)
                                    {
                                        allClaimed = false;
                                    }
                                }
                                if (allClaimed)
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
                                                TaskModified.Set();
                                            }
                                        }
                                        if (m_Error != null)
                                        {
                                            m_Error(this, ex);
                                        }
                                    }
                                }
                            }
                        }
                        //Wait until next task received or app is closed.
                        if (EventWaitHandle.WaitAny(new EventWaitHandle[] { info.NewTask, Closing }) == 1)
                        {
                            break;
                        }                        
                        info.ExecutingTask.Set();
                        taskinfo = null;                        
                        lock (info.ClaimedTasks)
                        {
                            foreach (GXClaimedTask it in info.ClaimedTasks)
                            {
                                if (it.Task.State == TaskState.Pending)
                                {
                                    it.Task.State = TaskState.Processing;
                                    taskinfo = it;
                                    System.Diagnostics.Debug.WriteLine("Processing task: " + it.Task.Id);
                                    break;
                                }
                            }                            
                        }

                        //If task is claimed.
                        if (taskinfo != null)
                        {
                            if (taskinfo.Task.TaskType == TaskType.MediaOpen)
                            {
                                IGXMedia media = null;
                                leaveConnectionOpen = true;
                                if (medias.ContainsKey(taskinfo.Settings))
                                {
                                    media = medias[taskinfo.Settings];
                                    media_OnMediaStateChange(media, new MediaStateEventArgs(MediaState.Open));
                                }
                                else
                                {
                                    try
                                    {
                                        media = new Gurux.Communication.GXClient().SelectMedia(taskinfo.Media);
                                        if (TraceLevel != System.Diagnostics.TraceLevel.Off)
                                        {
                                            media.Trace = TraceLevel;
                                            media.OnTrace += new TraceEventHandler(media_OnTrace);
                                        }
                                        media.Settings = taskinfo.Settings;
                                        media.OnReceived += new ReceivedEventHandler(media_OnReceived);
                                        media.OnMediaStateChange += new MediaStateChangeEventHandler(media_OnMediaStateChange);
                                        media.OnError += new Gurux.Common.ErrorEventHandler(media_OnError);
                                        media.Open();
                                        medias.Add(taskinfo.Settings, media);
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
                                TaskModified.Set();
                            }
                            else if (taskinfo.Task.TaskType == TaskType.MediaClose)
                            {
                                if (medias.ContainsKey(taskinfo.Settings))
                                {
                                    IGXMedia media = medias[taskinfo.Settings];
                                    medias.Remove(taskinfo.Settings);
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
                                TaskModified.Set();                                        
                            }
                            else if (taskinfo.Task.TaskType == TaskType.MediaWrite)
                            {
                                IGXMedia media;
                                if (!medias.ContainsKey(taskinfo.Settings))
                                {
                                    media = new Gurux.Communication.GXClient().SelectMedia(taskinfo.Media);
                                    medias.Add(taskinfo.Settings, media);
                                    media.Settings = taskinfo.Settings;
                                    media.OnReceived += new ReceivedEventHandler(media_OnReceived);
                                    media.OnMediaStateChange += new MediaStateChangeEventHandler(media_OnMediaStateChange);
                                    media.OnError += new Gurux.Common.ErrorEventHandler(media_OnError);
                                    media.Open();
                                }
                                else
                                {
                                    media = medias[taskinfo.Settings];
                                }
                                media.Send(Gurux.Common.GXCommon.HexToBytes(taskinfo.Data, false), null);
                                taskinfo.Task.State = TaskState.Succeeded;
                                TaskModified.Set();
                            }
                            else if (taskinfo.Task.TaskType == TaskType.MediaState ||
                                taskinfo.Task.TaskType == TaskType.MediaError)
                            {
                                //This might happend is DC is closed and there are event info left.
                                taskinfo.Task.State = TaskState.Succeeded;
                                TaskModified.Set();
                            }
                            else
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
                                        path = Path.Combine(path, "DeviceTemplates");
                                        if (!Directory.Exists(path))
                                        {
                                            Directory.CreateDirectory(path);
                                            Gurux.Common.GXFileSystemSecurity.UpdateDirectorySecurity(path);
                                        }
                                        path = Path.Combine(path, taskinfo.DeviceTemplate.ToString());
                                        //Load Device template if not loaded yet.                                 
                                        if (!Directory.Exists(path))
                                        {
                                            Directory.CreateDirectory(path);
                                            Gurux.Common.GXFileSystemSecurity.UpdateDirectorySecurity(path);
                                            byte[] data = DC.GetDeviceTemplateData(taskinfo.DeviceTemplate);
                                            pc.Import(data, path);
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
                                        TaskModified.Set();
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
                                        pc.ExecuteTransaction(taskinfo);
                                        taskinfo.Task.State = TaskState.Succeeded;
                                        TaskModified.Set();
                                    }
                                    else if (taskinfo.Task.TaskType == TaskType.StartMonitor)
                                    {
                                        leaveConnectionOpen = true;
                                        pc.StartMonitoring();
                                        taskinfo.Task.State = TaskState.Succeeded;
                                        TaskModified.Set();
                                    }
                                    else if (taskinfo.Task.TaskType == TaskType.StopMonitor)
                                    {
                                        leaveConnectionOpen = false;
                                        pc.StopMonitoring();
                                        taskinfo.Task.State = TaskState.Succeeded;
                                        TaskModified.Set();
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
                                        TaskModified.Set();
                                    }
                                    if (m_Error != null)
                                    {
                                        m_Error(this, ex);
                                    }
                                }
                            }
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
                            info.Exceptions.Add(taskinfo, ex);
                            taskinfo.Task.State = TaskState.Failed;
                            TaskModified.Set();
                        }
                    }
                    if (m_Error != null)
                    {
                        m_Error(this, ex);
                    }
                }
                finally
                {
                    info.ExecutingTask.Reset();
                }
            }
            pc.Close();
            //Unload app domain.
            System.AppDomain.Unload(td);
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
                DC.MediaError(taskinfo.Task, ex);
            }
        }

        /// <summary>
        /// Report Media state change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void media_OnMediaStateChange(object sender, MediaStateEventArgs e)
        {
            if (DC != null && (e.State == MediaState.Open || e.State == MediaState.Closed))
            {
                IGXMedia media = sender as IGXMedia;
                DC.MediaStateChange(DC.DataCollectorGuid, media.MediaType, media.Settings, e.State);
            }
        }

        void media_OnReceived(object sender, ReceiveEventArgs e)
        {
            IGXMedia media = sender as IGXMedia;
            DC.Write(DC.DataCollectorGuid, media.MediaType, media.Settings, (byte[])e.Data, 0, null);
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
            //Read exists tasks.
            GXAmiTask[] tasks = DC.GetTasks(TaskState.Pending);
            if (tasks.Length != 0)
            {
                DC_OnTasksAdded(DC, tasks);
            }
            List<Thread> Threads = new List<Thread>();
            while (!Closing.WaitOne(1))
            {
                //Wait until new task is added.
                TaskModified.WaitOne();
                if (!Closing.WaitOne(1))
                {
                    //Get new task
                    GXClaimedTask taskinfo = null;
                    lock (UnclaimedTasks)
                    {
                        taskinfo = null;
                        foreach (GXAmiTask task in UnclaimedTasks)
                        {
                            if (task.State == TaskState.Pending)
                            {
                                lock (UnclaimedTasks)
                                {
                                    bool idle = true;
                                    if (task.TargetDeviceID != 0)
                                    {
                                        idle = !ClaimedTasks.ContainsKey(task.TargetDeviceID) ||
                                            ClaimedTasks[task.TargetDeviceID].ClaimedTasks.Count == 0;
                                    }
                                    else
                                    {
                                        idle = !ClaimedTasks.ContainsKey(task.DataCollectorGuid) ||
                                            ClaimedTasks[task.DataCollectorGuid].ClaimedTasks.Count == 0;
                                    }
                                    //If collector is reading do not claim task before collector is finished reading.
                                    if (idle)
                                    {
                                        try
                                        {
                                            UnclaimedTasks.Remove(task);
                                            taskinfo = DC.ClaimTask(task);
                                            if (taskinfo != null)
                                            {
                                                if (m_TasksClaimed != null)
                                                {
                                                    task.DataCollectorID = taskinfo.DataCollectorID;
                                                    m_TasksClaimed(this, new GXAmiTask[] { task });
                                                }
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
                                }
                            }                            
                        }
                    }
                    if (taskinfo == null)
                    {
                        foreach (var c in ClaimedTasks)
                        {                            
                            foreach (GXClaimedTask it2 in c.Value.ClaimedTasks)
                            {
                                GXAmiTask task = it2.Task;
                                //Remove task after successful reading.
                                if (task.State == TaskState.Succeeded)
                                {
                                    lock (c.Value.ClaimedTasks)
                                    {
                                        c.Value.ClaimedTasks.Remove(it2);
                                    }
                                    try
                                    {
                                        System.Diagnostics.Debug.WriteLine("Remove task: " + it2.Task.Id);
                                        DC.RemoveTask(task, true);
                                        System.Diagnostics.Debug.WriteLine("Remove task succeeded: " + it2.Task.Id);
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
                                                //If collector is reading do not claim task before collector is finished reading.
                                                if (!ClaimedTasks.ContainsKey(ut.TargetDeviceID) ||
                                                    ClaimedTasks[ut.TargetDeviceID].ClaimedTasks.Count == 0)
                                                {
                                                    TaskModified.Set();
                                                }
                                                break;
                                            }
                                        }
                                    }
                                    System.Diagnostics.Debug.WriteLine("Wait next task.");
                                    break; //Break here because task is removed.                                    
                                }
                                else if (task.State == TaskState.Timeout)
                                {
                                    try
                                    {
                                        lock (c.Value.ClaimedTasks)
                                        {
                                            c.Value.ClaimedTasks.Remove(it2);
                                        }
                                        DC.AddDeviceError(task, new TimeoutException(), 1);
                                        //Remove failed task.
                                        DC.RemoveTask(task, true);
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
                                        lock (c.Value.ClaimedTasks)
                                        {
                                            c.Value.ClaimedTasks.Remove(it2);
                                        }
                                        foreach (var it in c.Value.Exceptions)
                                        {
                                            //If device is caused the error.
                                            if (it.Key.Task.TargetDeviceID != 0)
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
                                        }
                                        c.Value.Exceptions.Clear();
                                        //Remove failed task.
                                        DC.RemoveTask(task, true);
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
                    //If task is claimed.
                    if (taskinfo != null)
                    {
                        if (taskinfo.Task.TargetDeviceID != 0)
                        {
                            GXClaimedInfo dcinfo = null;
                            if (ClaimedTasks.ContainsKey(taskinfo.Task.TargetDeviceID))
                            {
                                dcinfo = ClaimedTasks[taskinfo.Task.TargetDeviceID];
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
                                ClaimedTasks.Add(taskinfo.Task.TargetDeviceID, dcinfo);
                                Thread thread = new Thread(new ParameterizedThreadStart(DeviceCollector));
                                thread.Start(dcinfo);
                                Threads.Add(thread);
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
                                Threads.Add(thread);
                            }
                        }
                    }
                }
            }
            //Wait until thereas are closed.
            foreach (Thread it in Threads)
            {
                it.Join();
            }
            Closed.Set();     
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
                    if (task.DataCollectorGuid != Guid.Empty && DC.DataCollectorGuid != task.DataCollectorGuid)
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
                    foreach (GXAmiTask t in UnclaimedTasks)
                    {
                        if (t.Id == task.Id)
                        {
                            UnclaimedTasks.Remove(t);
                            break;
                        }
                    }
                    if (task.DataCollectorGuid != this.Guid)
                    {
                        throw new Exception("Wrong Data Collector.");
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
                Closed.WaitOne(5000);
                if (Thread != null)
                {
                    Thread.Abort();
                }
                DC.Dispose();
                DC = null;
            }
        }
        #endregion        
    }
}
