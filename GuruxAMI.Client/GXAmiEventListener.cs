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

namespace GuruxAMI.Client
{
    /// <summary>
    /// Because DC is in own namespace we must create wrapper to handle events.
    /// </summary>
    class GXAmiEventListener : MarshalByRefObject
    {
        internal delegate void TraceAddEventHandler(GXAmiTrace[] traces);
        internal delegate void PropertyUpdateEventHandler(GXAmiDataValue[] values);
        internal delegate void UpdateparametersEventHandler(GXAmiParameter[] parameters);
        internal delegate void ErrorEventHandler(GXAmiTask task, Exception ex);
        internal delegate void DeviceStateChangedEventHandler(ulong deviceId, Gurux.Device.DeviceStates states);

        GXAmiDataCollectorServer Server;
        GXAmiClient DC;
        public GXAmiEventListener(GXAmiDataCollectorServer server, GXProxyClass pc, GXAmiClient dc)
        {
            Server = server;
            DC = dc;
            pc.OnUpdated += new PropertyUpdateEventHandler(OnUpdated);
            pc.OnError += new ErrorEventHandler(pc_OnError);
            pc.OnStateChange += new DeviceStateChangedEventHandler(pc_OnStateChange);
            pc.OnUpdateParameters += new UpdateparametersEventHandler(pc_OnUpdateParameters);
            pc.OnTrace += new TraceAddEventHandler(pc_OnTrace);
        }

        void pc_OnTrace(GXAmiTrace[] traces)
        {
            DC.AddTraces(traces);
        }

        void pc_OnUpdateParameters(GXAmiParameter[] parameters)
        {
            DC.Update(parameters);
        }

        void pc_OnStateChange(ulong deviceId, Gurux.Device.DeviceStates states)
        {
            DC.UpdateDeviceState(deviceId, states);
        }

        void pc_OnError(GXAmiTask task, Exception ex)
        {
            if (Server.m_Error != null)
            {
                Server.m_Error(this, ex);
            }
            DC.AddDeviceError(task, ex, 1);
        }

        void OnUpdated(GXAmiDataValue[] values)
        {
            DC.UpdateValues(values);
        }
    }
}
