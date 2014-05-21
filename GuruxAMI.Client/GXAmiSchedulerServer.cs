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
using GuruxAMI.Common;
using GuruxAMI.Common.Messages;
using Gurux.Device.Editor;
using System.IO;
using System.Net.NetworkInformation;
using Gurux.Device;
using System.Threading;
using System.Reflection;
using Gurux.Common;
#if !SS4
using ServiceStack.ServiceClient.Web;
using Quartz;
using Quartz.Impl;
#else
using ServiceStack;
using System.Threading.Tasks;
#endif

namespace GuruxAMI.Client
{
    class GXScheduleJob : IJob
    {
        void IJob.Execute(IJobExecutionContext context)
        {
            GXAmiClient client = context.JobDetail.JobDataMap["Client"] as GXAmiClient;
            ulong id = (ulong)context.JobDetail.JobDataMap["Target"];
            GXAmiDevice device = new GXAmiDevice();
            //Get schedule
            GXAmiSchedule schedule = client.GetSchedule(id);

            //Notify that schedule task is started.            
            client.NotifyScheduleStateChange(schedule, ScheduleState.TaskStart);
            //Add tasks.            
            foreach (GXAmiScheduleTarget t in schedule.Targets)
            {
                if (t.TargetType == TargetType.Device)
                {
                    device.Id = t.TargetID;
                    client.Read(device);
                }
            }            
            //Notify that schedule task is ended.
            client.NotifyScheduleStateChange(schedule, ScheduleState.TaskFinish);
        }
    }

    /// <summary>
    /// Scheduler server is used to handle GuruxAMI schedule events.
    /// </summary>
    public class GXAmiSchedulerServer : IDisposable
    {
        StdSchedulerFactory m_SchedulerFactory;
        IScheduler m_sched;

        Dictionary<ulong, GXAmiSchedule> Schedules = null;
        GXAmiClient Client;
        SchedulesAddedEventHandler m_ScheduleAdded;
        SchedulesUpdatedEventHandler m_SchedulesUpdated;
        SchedulesRemovedEventHandler m_SchedulesRemoved;
        SchedulesStateChangedEventHandler m_ScheduleStateChanged;
        bool sharedClient;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="baseUr">Address of GuruxAMI server.</param>
        public GXAmiSchedulerServer(string baseUr, string userName, string password)
        {
            sharedClient = false;
            Client = new GXAmiClient(baseUr, userName, password);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="client"></param>
        public GXAmiSchedulerServer(GXAmiClient client)
        {
            Client = client;
            sharedClient = true;
        }        

        /// <summary>
        /// Start schedules.
        /// </summary>
        public void Start()
        {
            if (m_SchedulerFactory == null)
            {
                m_SchedulerFactory = new StdSchedulerFactory();
                m_sched = m_SchedulerFactory.GetScheduler();
                // Set up the listener
                //TODO: m_sched.ListenerManager.AddJobListener(new GXScheduleListener());
                m_sched.Start();
            }

            Client.OnSchedulesAdded += new SchedulesAddedEventHandler(Client_OnSchedulesAdded);
            Client.OnSchedulesRemoved += new SchedulesRemovedEventHandler(Client_OnSchedulesRemoved);
            Client.OnSchedulesUpdated += new SchedulesUpdatedEventHandler(Client_OnSchedulesUpdated);
            Client.OnSchedulesStateChanged += new SchedulesStateChangedEventHandler(Client_OnSchedulesStateChanged);
            if (!sharedClient)
            {
                Client.StartListenEvents();
            }
            Schedules = new Dictionary<ulong, GXAmiSchedule>();
            foreach (GXAmiSchedule it in Client.GetSchedules())
            {
                Schedules.Add(it.Id, it);
            }
        }


        /// <summary>
        /// Stop schedules.
        /// </summary>
        public void Stop()
        {
            if (m_sched != null)
            {
                m_sched.Shutdown(true);
                m_sched = null;
                m_SchedulerFactory = null;
            }
            Client.OnSchedulesAdded -= new SchedulesAddedEventHandler(Client_OnSchedulesAdded);
            Client.OnSchedulesRemoved -= new SchedulesRemovedEventHandler(Client_OnSchedulesRemoved);
            Client.OnSchedulesUpdated -= new SchedulesUpdatedEventHandler(Client_OnSchedulesUpdated);
            Client.OnSchedulesStateChanged -= new SchedulesStateChangedEventHandler(Client_OnSchedulesStateChanged);
            if (!sharedClient)
            {
                Client.StopListenEvents();
            }
        }

        /// <summary>
        /// New schedule(s) are added.
        /// </summary>
        public event SchedulesAddedEventHandler OnSchedulesAdded
        {
            add
            {

                m_ScheduleAdded += value;
            }
            remove
            {
                m_ScheduleAdded -= value;
            }
        }

        /// <summary>
        /// Schedule(s) are updated.
        /// </summary>
        public event SchedulesUpdatedEventHandler OnSchedulesUpdated
        {
            add
            {
                m_SchedulesUpdated += value;
            }
            remove
            {
                m_SchedulesUpdated -= value;
            }
        }

        /// <summary>
        /// Schedule(s) are removed.
        /// </summary>
        public event SchedulesRemovedEventHandler OnSchedulesRemoved
        {
            add
            {
                m_SchedulesRemoved += value;
            }
            remove
            {
                m_SchedulesRemoved -= value;
            }
        }

        /// <summary>
        /// Schedule(s) states are changed.
        /// </summary>
        public event SchedulesStateChangedEventHandler OnSchedulesStateChanged
        {
            add
            {
                m_ScheduleStateChanged += value;
            }
            remove
            {
                m_ScheduleStateChanged -= value;
            }
        }

        /// <summary>
        /// Schedule item is updated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="schedules"></param>
        void Client_OnSchedulesUpdated(object sender, GXAmiSchedule[] schedules)
        {
            lock (Schedules)
            {
                foreach (GXAmiSchedule it in schedules)
                {
                    Schedules[it.Id] = it;
                }
            }
            if (m_SchedulesUpdated != null)
            {
                m_SchedulesUpdated(sender, schedules);
            }
        }

        /// <summary>
        /// Schedule item is removed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="schedules"></param>
        void Client_OnSchedulesRemoved(object sender, ulong[] schedules)
        {
            lock (Schedules)
            {
                foreach (ulong it in schedules)
                {
                    Schedules.Remove(it);
                }
            }
            if (m_SchedulesRemoved != null)
            {
                m_SchedulesRemoved(sender, schedules);
            }
        }

        /// <summary>
        /// New schedule item is added.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="schedules"></param>
        void Client_OnSchedulesAdded(object sender, GXAmiSchedule[] schedules)
        {
            lock (Schedules)
            {
                foreach (GXAmiSchedule it in schedules)
                {
                    Schedules.Add(it.Id, it);
                }
            }
            if (m_ScheduleAdded != null)
            {
                m_ScheduleAdded(sender, schedules);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// see more: http://www.cronmaker.com/
        /// </remarks>
        /// <returns></returns>
        internal ITrigger GetTrigger(GXAmiSchedule schedule)
        {
            string format = null;
            DateTime end;
            ITrigger trigger = null;
            if (schedule.RepeatMode == ScheduleRepeat.Once)
            {
                trigger = (ITrigger)TriggerBuilder.Create()
                    .StartNow()
                    .EndAt(DateTime.Now)
                    .Build();
            }
            else if (schedule.RepeatMode == ScheduleRepeat.Second)
            {
                format = "0/" + schedule.Interval.ToString() + " * * * * ?";
            }
            else if (schedule.RepeatMode == ScheduleRepeat.Minute)
            {
                format = "0 0/" + schedule.Interval.ToString() + " * * * ?";
            }
            else if (schedule.RepeatMode == ScheduleRepeat.Hour)
            {
                format = "0 0 0/" + schedule.Interval.ToString() + " * * ?";
            }
            else if (schedule.RepeatMode == ScheduleRepeat.Day)
            {
                format = string.Format("{3} {2} {1} 1/{0} * ?",
                            schedule.Interval,
                            schedule.TransactionStartTime.Value.Hour,
                            schedule.TransactionStartTime.Value.Minute,
                            schedule.TransactionStartTime.Value.Second);
            }
            else if (schedule.RepeatMode == ScheduleRepeat.Week)
            {
                end = DateTime.MaxValue;
                if (schedule.ScheduleEndTime != null)
                {
                    end = new DateTime(schedule.ScheduleEndTime.Value.Year, schedule.ScheduleEndTime.Value.Month,
                        schedule.ScheduleEndTime.Value.Day, schedule.TransactionEndTime.Value.Hour, 
                        schedule.TransactionEndTime.Value.Minute, schedule.TransactionEndTime.Value.Second);
                }
                List<string> days = new List<string>();
                foreach (DayOfWeek it in schedule.DayOfWeeks)
                {
                    days.Add(it.ToString().Substring(0, 3).ToUpper());
                }
                string tmp = string.Format("{3} {2} {1} ? * {0}, *",
                                            string.Join(",", days.ToArray()),
                                            schedule.TransactionStartTime.Value.Hour,
                                            schedule.TransactionStartTime.Value.Minute,
                                            schedule.TransactionStartTime.Value.Second);
                IScheduleBuilder scheduleBuilder = CronScheduleBuilder.CronSchedule(tmp);
                trigger = (ITrigger)TriggerBuilder.Create()
                .WithIdentity(schedule.Name)
                .WithSchedule(scheduleBuilder)
                .Build();
            }
            else if (schedule.RepeatMode == ScheduleRepeat.Month)
            {
                string tmp = string.Format("0 {2} {1} {0} 1/{3} ? *",
                                            schedule.DayOfMonth,
                                            schedule.TransactionStartTime.Value.Hour,
                                            schedule.TransactionStartTime.Value.Minute,
                                            schedule.Interval);

                IScheduleBuilder scheduleBuilder = CronScheduleBuilder.CronSchedule(tmp);
                trigger = (ITrigger)TriggerBuilder.Create()
                .WithIdentity(schedule.Name)
                .WithSchedule(scheduleBuilder)
                .Build();
            }
            if (trigger == null)
            {
                end = DateTime.MaxValue;
                if (schedule.ScheduleEndTime != null)
                {
                    end = new DateTime(schedule.ScheduleEndTime.Value.Year, schedule.ScheduleEndTime.Value.Month,
                        schedule.ScheduleEndTime.Value.Day, schedule.TransactionEndTime.Value.Hour,
                        schedule.TransactionEndTime.Value.Minute, schedule.TransactionEndTime.Value.Second);
                }
                IScheduleBuilder scheduleBuilder = CronScheduleBuilder.CronSchedule(format);//.InTimeZone(TimeZoneInfo.Local);
                trigger = (ITrigger)TriggerBuilder.Create()
                .WithIdentity(schedule.Name)
                .WithSchedule(scheduleBuilder)
                .Build();
            }
            return trigger;
        }

        /// <summary>
        /// Schedule is started or stopped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="schedules"></param>
        void Client_OnSchedulesStateChanged(object sender, GXAmiSchedule[] schedules)
        {
            //Update schedule state.
            lock (Schedules)
            {
                GXAmiDevice device = new GXAmiDevice();
                foreach (GXAmiSchedule it in schedules)
                {
                    if (Schedules[it.Id].Status != it.Status)
                    {
                        Schedules[it.Id].Status = it.Status;
                        if (it.Status == ScheduleState.Run)
                        {
                            foreach (GXAmiScheduleTarget t in Schedules[it.Id].Targets)
                            {
                                if (t.TargetType == TargetType.Device)
                                {               
                                    device.Id = t.TargetID;
                                    Client.Read(device);
                                }
                            }
                        }
                        else if (it.Status == ScheduleState.Start)
                        {
                            System.Collections.Generic.IDictionary<string, object> data = new Dictionary<string, object>();
                            data.Add(new KeyValuePair<string, object>("Client", Client));
                            //Target is added by ID because devices can be added or removed from it.
                            data.Add(new KeyValuePair<string, object>("Target", it.Id));
                            JobKey id = new JobKey(it.Name + it.Id.ToString(), it.Name + it.Id.ToString());
                            IJobDetail job = JobBuilder.Create(typeof(GXScheduleJob)).WithIdentity(new JobKey(it.Name + it.Id.ToString(), it.Name + it.Id.ToString()))
                                .WithIdentity(id)
                                .SetJobData(new JobDataMap(data))
                                .Build();
                            ITrigger t = GetTrigger(it);
                            m_sched.ScheduleJob(job, t);
                        }
                        else if (it.Status == ScheduleState.End)
                        {
                            if ((it.Status & ScheduleState.Run) != 0)
                            {
                                it.Status = ScheduleState.None;
                                m_sched.DeleteJob(new JobKey(it.Name + it.Id.ToString(), it.Name + it.Id.ToString()));
                            }
                        }
                    }
                }
            }
            if (m_ScheduleStateChanged != null)
            {
                m_ScheduleStateChanged(sender, schedules);
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}