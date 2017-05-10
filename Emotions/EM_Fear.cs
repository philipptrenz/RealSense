﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RealSense.Emotions
{
    class EM_Fear : RSModule
    {
        // Default values
        public EM_Fear()
        {
            debug = true;
        }

        /*
         *  1 = inner brow raised -> BrowShift
            2 = outer brow raised -> BrowShift
            4 = brow lowered -> BrowShift
            5 = upper lid raised -> EyelidTight
            6 = cheeck raised -> CheeckRaised (not working)
            7 = lid tightened -> EyelidTight
            9 = nose wrinkled -> NoseWrinkled
            12 = lip corner pulled (up) -> LipCorner
            14 = grübchen -> none
            15 = lip corner lowered -> LipLine
            16 = lower lip lowered ->LowerLipLowered
            20 = lip stretched -> LipStretched
            23 = lip tightened -> LipsTightened
            26 = jaw drop -> JawDrap

            Verachtung (12 (R,L), 14(R,L)
            Trauer (1,4,15,(20?)
            Wut (4,5,6,23)
            Ekel (9,15,16,4)
            Überraschung (1,2,5B,26)
            Freude (6,12, 7)
            Angst (1,2,4,5,6,20,26)

         * */

        public override void Work(Graphics g)
        {
            //Fear --> BrowShift, EyelidTight, LipStreched, JawDrop

            //percentage Fear
            int p_brow = 30;
            int p_eye = 35;
            int p_lip = 20;
            int p_jaw = 15;

            //brow Value 0-100
            double temp_left = model.AU_Values[typeof(ME_BrowShift).ToString() + "_left"];
            double temp_right = model.AU_Values[typeof(ME_BrowShift).ToString() + "_right"];
            double browValue = temp_left < temp_right ? temp_left : temp_right;
            browValue = browValue * p_brow / 100;

            //eye Value 0-100
            double eyeValue = model.AU_Values[typeof(ME_EyelidTight).ToString()];
            eyeValue = eyeValue * p_eye / 100;

            //lipLine Value 0-100
            double lipValue = model.AU_Values[typeof(ME_LipStretched).ToString()];
            lipValue = lipValue * p_lip / 100;

            //jaw 0-100
            double jawValue = model.AU_Values[typeof(ME_JawDrop).ToString()];
            jawValue = jawValue * p_jaw / 100;

            double fear = browValue + eyeValue + lipValue + jawValue;
            fear = fear > 0 ? fear : 0;
            model.Emotions["Fear"] = fear;

            // print debug-values 
            if (debug)
            {
                output = "Fear: " + fear;
            }

        }
    }
}









