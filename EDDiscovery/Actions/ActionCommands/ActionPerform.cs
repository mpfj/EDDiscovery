﻿/*
 * Copyright © 2017 EDDiscovery development team
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this
 * file except in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software distributed under
 * the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
 * ANY KIND, either express or implied. See the License for the specific language
 * governing permissions and limitations under the License.
 * 
 * EDDiscovery is not affiliated with Fronter Developments plc.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EDDiscovery.Actions
{
    public class ActionPerform : Action
    {
        public override bool AllowDirectEditingOfUserData { get { return true; } }

        public override bool ConfigurationMenu(Form parent, EDDiscoveryForm discoveryform, List<string> eventvars)
        {
            string promptValue = PromptSingleLine.ShowDialog(parent, discoveryform.theme, "Perform command", UserData, "Configure Perform Command");
            if (promptValue != null)
            {
                userdata = promptValue;
            }

            return (promptValue != null);
        }

        public override bool ExecuteAction(ActionProgramRun ap)
        {
            string res;
            if (ap.functions.ExpandString(UserData, ap.currentvars, out res) != ConditionLists.ExpandResult.Failed)
            {
                StringParser sp = new StringParser(res);
                string cmdname = sp.NextWord(" ", true);

                if (cmdname == null)
                {
                    ap.ReportError("Missing command in Perform");
                }
                else if (cmdname.Equals("3dmap"))
                {
                    ap.actioncontroller.DiscoveryForm.Open3DMap(null);
                }
                else if (cmdname.Equals("2dmap"))
                {
                    ap.actioncontroller.DiscoveryForm.Open2DMap();
                }
                else if (cmdname.Equals("edsm"))
                {
                    EDDiscovery2.EDSM.EDSMClass edsm = new EDDiscovery2.EDSM.EDSMClass();
                    ap.actioncontroller.DiscoveryForm.EdsmSync.StartSync(edsm, EDDiscoveryForm.EDDConfig.CurrentCommander.SyncToEdsm, EDDiscoveryForm.EDDConfig.CurrentCommander.SyncFromEdsm, EDDiscovery2.EDDConfig.Instance.DefaultMapColour);
                }
                else if (cmdname.Equals("refresh"))
                {
                    ap.actioncontroller.DiscoveryForm.RefreshHistoryAsync(checkedsm: true);
                }
                else if (cmdname.Equals("url"))
                {
                    string url = sp.LineLeft;

                    if (url.StartsWith("http:", StringComparison.InvariantCultureIgnoreCase) || url.StartsWith("https:", StringComparison.InvariantCultureIgnoreCase))        // security..
                    {
                        System.Diagnostics.Process.Start(url);
                    }
                    else
                        ap.ReportError("Perform url must start with http");
                }
                else if (cmdname.Equals("configurevoice"))
                    ap.actioncontroller.ConfigureVoice();
                else if (cmdname.Equals("manageaddons"))
                    ap.actioncontroller.ManageAddOns();
                else if (cmdname.Equals("editaddons"))
                    ap.actioncontroller.EditAddOnActionFile();
                else
                    ap.ReportError("Unknown command " + cmdname + " in Performaction");
            }
            else
                ap.ReportError(res);

            return true;
        }
    }
}
