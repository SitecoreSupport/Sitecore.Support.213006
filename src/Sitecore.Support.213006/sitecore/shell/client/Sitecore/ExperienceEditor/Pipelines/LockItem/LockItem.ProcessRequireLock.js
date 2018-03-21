define(["sitecore", "/-/speak/v1/ExperienceEditor/ExperienceEditor.js", "/-/speak/v1/ExperienceEditor/TranslationUtil.js"], function (Sitecore, ExperienceEditor, TranslationUtil) {
    return {
        priority: 1,
        execute: function (context) {
            var instance = ExperienceEditor.getContext().instance;
            if (!instance.currentContext.requireLockBeforeEdit) {
                return;
            }

            if (instance.currentContext.isLocked) {
                // Updates notification after locking. Related to the bug #213006
                instance.initializeNotifications(instance);

                ExperienceEditor.Common.removeNotificationMessage(TranslationUtil.translateText(TranslationUtil.keys.You_must_lock_this_item_before_you_can_edit_it));
                instance.setHeight();
            } else {
                // Updates notification after unlocking. Related to the bug #213006
                instance.initializeNotifications(instance);

                instance.processItemLocking(instance);
                ExperienceEditor.getPageEditingWindow().Sitecore.PageModes.ChromeManager.hoverFrame().deactivate();
            }

            var lockCommandDependency = instance.LockCommandDependency;
            if (!lockCommandDependency) {
                return;
            }

            lockCommandDependency.viewModel.runDependenciesCanExecute(context);
        }
    };
});