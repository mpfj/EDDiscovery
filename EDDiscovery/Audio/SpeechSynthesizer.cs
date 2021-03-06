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

namespace EDDiscovery.Audio
{
    public interface ISpeechEngine
    {
        string[] GetVoiceNames();
        System.IO.MemoryStream Speak(string phrase, string voice , int volume, int rate);
    }

    public class SpeechSynthesizer
    {
        ISpeechEngine speechengine;
        Random rnd = new Random();

        public SpeechSynthesizer( ISpeechEngine engine )
        {
            speechengine = engine;
        }

        public string[] GetVoiceNames()
        {
            return speechengine.GetVoiceNames();
        }

        public string ToPhrase(string phraselist, out string errlist, ConditionFunctions f = null, ConditionVariables curvars = null)
        {
            string res = phraselist;
            if (f == null || f.ExpandString(phraselist, curvars, out res) != EDDiscovery.ConditionLists.ExpandResult.Failed)       //Expand out.. and if no errors
            {
                string[] phrasearray = res.Split(';');

                if (phrasearray.Length > 1)     // if we have at least x;y
                {
                    if (phrasearray[0].Length == 0 && phrasearray.Length >= 2)   // first empty, and we have two or more..
                    {
                        res = phrasearray[1];           // say first one
                        if (phrasearray.Length > 2)   // if we have ;first;second;third, pick random at then
                        {
                            res += phrasearray[2 + rnd.Next(phrasearray.Length - 2)];
                        }
                    }
                    else
                        res = phrasearray[rnd.Next(phrasearray.Length)];    // pick randomly
                }

                errlist = null;
                return res;
            }
            else
            {
                errlist = res;
                return null;
            }
        }

        public System.IO.MemoryStream Speak(string say, string voice, int rate)
        {
            return speechengine.Speak(say, voice, 100, rate);     // samples are always generated at 100 volume
        }
    }
}
