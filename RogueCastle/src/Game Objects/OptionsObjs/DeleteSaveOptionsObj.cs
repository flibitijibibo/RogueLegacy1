using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class DeleteSaveOptionsObj : OptionsObj
    {
        public DeleteSaveOptionsObj(OptionsScreen parentScreen)
            : base(parentScreen, "LOC_ID_DELETE_SAVE_OPTIONS_1") //"Delete Save"
        {
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
                    manager.DialogueScreen.SetDialogue("Delete Save");
                    manager.DialogueScreen.SetDialogueChoice("ConfirmTest1");
                    manager.DialogueScreen.SetConfirmEndHandler(this, "DeleteSaveAskAgain");
                    manager.DialogueScreen.SetCancelEndHandler(this, "CancelCommand");
                    manager.DisplayScreen(ScreenType.Dialogue, false, null);
                }
            }
        }

        public void CancelCommand()
        {
            IsActive = false;
        }

        public void DeleteSaveAskAgain()
        {
            RCScreenManager manager = m_parentScreen.ScreenManager as RCScreenManager;
            manager.DialogueScreen.SetDialogue("Delete Save2");
            manager.DialogueScreen.SetDialogueChoice("ConfirmTest1");
            manager.DialogueScreen.SetConfirmEndHandler(this, "DeleteSave");
            manager.DialogueScreen.SetCancelEndHandler(this, "CancelCommand");
            manager.DisplayScreen(ScreenType.Dialogue, false, null);
        }

        public void DeleteSave()
        {
            IsActive = false;
            Game.PlayerStats.Dispose();
            (m_parentScreen.ScreenManager.Game as Game).SaveManager.ClearAllFileTypes(false);
            (m_parentScreen.ScreenManager.Game as Game).SaveManager.ClearAllFileTypes(true);
            SkillSystem.ResetAllTraits();
            Game.PlayerStats = new PlayerStats();
            (m_parentScreen.ScreenManager as RCScreenManager).Player.Reset();
            SoundManager.StopMusic(1);

            (m_parentScreen.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.TutorialRoom, true);
            //(m_parentScreen.ScreenManager as RCScreenManager).HideCurrentScreen();
        }
    }
}
