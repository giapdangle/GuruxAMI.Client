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

using Gurux.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GuruxAMI.Client
{
    partial class GXAmiLoginForm : Form
    {
        GXAmiClientLoginInfo Info;
        GXAsyncWork work;        

        /// <summary>
        /// Get connected client.
        /// </summary>
        /// <remarks>
        /// If connection fails client is null.
        /// </remarks>
        public GXAmiClient Client
        {
            get;
            private set;
        }

        /// <summary>
        /// Show occurred error.
        /// </summary>
        void OnError(object sender, Exception ex)
        {
            Gurux.Common.GXCommon.ShowError(this, Info.Title, ex);
        }

        public GXAmiLoginForm(GXAmiClientLoginInfo info)
        {
            InitializeComponent();
            Info = info;
            work = new GXAsyncWork(this, AsyncStateChangeEventHandler, Connect, OnError, null, null);
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            this.NameTB.Text = Info.UserName;
            PasswordTB.Text = Info.Password;
            this.HostTB.Text = Info.Address;
            Info.RememberMe = this.RemembeMeCB.Checked = !string.IsNullOrEmpty(Info.UserName);
            if (Info.RememberMe)
            {
                LoginBtn_Click(this, null);
            }
        }

        void Connect(object sender, GXAsyncWork work, object[] parameters)
        {
            GXAmiClient cl = new GXAmiClient(Info.Address, this.NameTB.Text, this.PasswordTB.Text);
            cl.GetUserInfo();
            if (!work.IsCanceled)
            {
                Client = cl;
            }
        }

        /// <summary>
        /// Status of work is changed.
        /// </summary>
        void AsyncStateChangeEventHandler(object sender, GXAsyncWork work, object[] parameters, AsyncState state, string text)
        {
            bool isTrying = state == AsyncState.Start;
            this.NameTB.ReadOnly = this.PasswordTB.ReadOnly = isTrying;
            RemembeMeCB.Enabled = this.EditHostBtn.Enabled = this.LoginBtn.Enabled = !isTrying;
            //Close Wnd if connection succeeded.
            if (Client != null)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                Info.RememberMe = RemembeMeCB.Checked;
                Info.UserName = this.NameTB.Text;
                Info.Password = PasswordTB.Text;
                Close();
            }
        }

        /// <summary>
        /// Start login.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginBtn_Click(object sender, EventArgs e)
        {
            work.Start();   
        }

        /// <summary>
        /// Wait until work is closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelBtn_Click(object sender, EventArgs e)
        {
            //Wnd is not closed if connection is try to made.
            if (work.IsRunning)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.None;
            }            
            work.Cancel();
        }

        /// <summary>
        /// Edit GuruxAMI host settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditHostBtn_Click(object sender, EventArgs e)
        {
            HostForm dlg = new HostForm();
            dlg.Address = HostTB.Text;
            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                Info.Address = HostTB.Text = dlg.Address;
            }
        }

    }
}
