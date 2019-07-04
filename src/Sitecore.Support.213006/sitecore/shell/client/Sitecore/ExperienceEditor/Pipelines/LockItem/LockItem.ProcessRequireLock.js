﻿define(["sitecore", "/-/speak/v1/ExperienceEditor/ExperienceEditor.js", "/-/speak/v1/ExperienceEditor/TranslationUtil.js"], function (Sitecore, ExperienceEditor, TranslationUtil) {
  return {
    priority: 1,
    execute: function (context) {
      var instance = context.app;
      if (!instance.currentContext.requireLockBeforeEdit) {
        return;
      }

      if (instance.currentContext.isLocked) {
<<<<<<< HEAD
	
=======

>>>>>>> 36b1a52560425c0c3dda59b955aad79fcad99060
        ExperienceEditor.Common.removeNotificationMessage(TranslationUtil.translateText(TranslationUtil.keys.You_must_lock_this_item_before_you_can_edit_it));

        instance.setHeight();
      } else {
        ExperienceEditor.PipelinesUtil.initAndExecutePipeline(context.app.ProcessItemRequireLockPipeline, ExperienceEditor.RibbonApp.getAppContext(null, context.app));
<<<<<<< HEAD
		
=======
>>>>>>> 36b1a52560425c0c3dda59b955aad79fcad99060
      }

      var lockCommandDependency = instance.LockCommandDependency;
      if (!lockCommandDependency) {
        return;
      }

      lockCommandDependency.viewModel.runDependenciesCanExecute(context);
    }
  };
});