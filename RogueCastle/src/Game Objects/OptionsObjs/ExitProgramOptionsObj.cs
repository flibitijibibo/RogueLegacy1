using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class ExitProgramOptionsObj : OptionsObj
    {
        public ExitProgramOptionsObj(OptionsScreen parentScreen)
            : base(parentScreen, "LOC_ID_EXIT_ROGUE_LEGACY_OPTIONS_1") //"Quit Rogue Legacy"
        {
        }

        public void QuitProgram()
        {
            (m_parentScreen.ScreenManager.Game as Game).SaveOnExit();
            m_parentScreen.ScreenManager.Game.Exit();
        }

        public void CancelCommand()
        {
            IsActive = false;
        }

        public override bool IsActive
        {
            get { return base.IsActive; }
            set
            {
                base.IsActive = value;
                if (IsActive == true)
                {
                    RCScreenManager manager = m_parentScreen.ScreenManager as RCScreenManager;
                    manager.DialogueScreen.SetDialogue("Quit Rogue Legacy");
                    manager.DialogueScreen.SetDialogueChoice("ConfirmTest1");
                    manager.DialogueScreen.SetConfirmEndHandler(this, "QuitProgram");
                    manager.DialogueScreen.SetCancelEndHandler(this, "CancelCommand");
                    manager.DisplayScreen(ScreenType.Dialogue, false, null);
                }
            }
        }
    }
}
