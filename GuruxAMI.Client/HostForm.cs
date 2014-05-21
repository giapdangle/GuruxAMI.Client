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
using System.Windows.Forms;

namespace GuruxAMI.Client
{
	partial class HostForm: Form
	{
		public HostForm()
		{
			InitializeComponent();            
		}

        private void HostForm_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Address))
            {
                Address = Address.Replace("http://", "");
                WebCB.Checked = !Address.Contains(":");
                if (WebCB.Checked)
                {
                    CheckedChanged(null, null);
                    WebAddressTB.Text = Address;
                    HostTB.Text = "localhost";
                    PortTB.Text = "1337";
                }
                else
                {
                    WebAddressTB.Text = "http://www.gurux.org/GuruxAMI";
                    ServiceCB.Checked = true;
                    string[] tmp = Address.Split(new char[] { ':' });
                    if (tmp.Length == 2)
                    {
                        HostTB.Text = tmp[0];
                        PortTB.Text = tmp[1].Replace("/", "");
                    }
                    else
                    {
                        WebCB.Checked = true;
                    }
                }
            }
            else
            {
                WebAddressTB.Text = "http://www.gurux.org/GuruxAMI";
            }
        }

        public string Address
        {
            get;
            set;
        }

        private void OKBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (WebCB.Checked)
                {
                    if (WebAddressTB.Text.Length == 0)
                    {
                        throw new Exception("Invalid web address.");
                    }
                    Address = WebAddressTB.Text;
                    if (!Address.StartsWith("http://"))
                    {
                        Address += "http://";
                    }
                    if (!Address.EndsWith("/"))
                    {
                        Address += "/";
                    }
                }
                else
                {
                    if (HostTB.Text.Length == 0)
                    {
                        throw new Exception("Invalid host name.");
                    }
                    int port;
                    if (!int.TryParse(PortTB.Text, out port) || port == 0)
                    {
                        throw new Exception("Invalid port number.");
                    }
                    Address = "http://" + HostTB.Text + ":" + PortTB.Text + "/";
                }
            }
            catch (Exception ex)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.None;
                Gurux.Common.GXCommon.ShowError(this, ex);                
            }
        }

        private void CheckedChanged(object sender, EventArgs e)
        {
            bool useWeb = WebCB.Checked;
            WebAddressTB.ReadOnly = !useWeb;
            WebAddressTB.TabStop = useWeb;
            HostTB.ReadOnly = PortTB.ReadOnly = useWeb;
            HostTB.TabStop = PortTB.TabStop = !useWeb;
        }
	}
}
