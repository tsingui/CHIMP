﻿using Chimp.Actions;
using Chimp.ViewModels;
using System.Collections.Generic;

namespace Chimp.Providers.Action
{
    sealed class ScriptableActionProvider : ActionProvider
    {
        public ScriptableActionProvider(MainViewModel mainViewModel, IServiceActivator serviceActivator)
            : base(mainViewModel, serviceActivator)
        {
        }

        public override IEnumerable<IAction> GetActions()
        {
            if (CardViewModel.SelectedItem.Scriptable)
                yield return ServiceActivator.Create<ClearScriptableAction>();
            else if (CanSetScriptable)
                yield return ServiceActivator.Create<SetScriptableAction>();
        }

        private bool CanSetScriptable =>
            CardViewModel.SelectedItem.Switched != true
            && CardViewModel.SelectedItem.Bootable == null;
    }
}
