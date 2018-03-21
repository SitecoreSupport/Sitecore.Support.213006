using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.ExperienceEditor;
using Sitecore.ExperienceEditor.Utils;
using Sitecore.Globalization;
using Sitecore.SecurityModel;
using Sitecore.Shell.Applications.ContentManager.Panels;
using Sitecore.Shell.Framework.CommandBuilders;
using Sitecore.Workflows;
using System;
using Sitecore.Pipelines.GetPageEditorNotifications;

namespace Sitecore.Support.Pipelines.GetPageEditorNotifications
{
  public class GetWorkflowNotification : GetPageEditorNotificationsProcessor
  {
    // Methods
    private static string GetDescription(IWorkflow workflow, WorkflowState state, Database database) =>
      WorkflowUtility.GetWorkflowStateDescription(workflow, state, database,
        "The item is in the '{0}' workflow state in the '{1}' workflow.");
    private static bool CanShowCommands(Item item, WorkflowCommand[] commands)
    {
      Assert.ArgumentNotNull(item, "item");
      bool flag = Context.User.IsAdministrator || item.Locking.HasLock();
      return WorkflowPanel.CanShowCommands(item, commands) && flag;
    }


    public override void Process(GetPageEditorNotificationsArgs arguments)
    {
      Assert.ArgumentNotNull(arguments, "arguments");
      if (!WebUtility.IsEditAllVersionsTicked())
      {
        using (new SecurityDisabler())
        {
          Item contextItem = arguments.ContextItem;
          Database database = contextItem.Database;
          IWorkflowProvider workflowProvider = database.WorkflowProvider;
          if (workflowProvider != null)
          {
            IWorkflow workflow = workflowProvider.GetWorkflow(contextItem);
            if (workflow != null)
            {
              WorkflowState state = workflow.GetState(contextItem);
              if (state != null)
              {
                using (new LanguageSwitcher(WebUtility.ClientLanguage))
                {
                  WorkflowCommand[] commandArray;
                  string description = GetDescription(workflow, state, database);
                  string icon = state.Icon;
                  PageEditorNotification item =
                    new PageEditorNotification(description, PageEditorNotificationType.Information)
                    {
                      Icon = icon
                    };
                  using (new SecurityEnabler())
                  {
                    commandArray =
                      WorkflowFilterer.FilterVisibleCommands(workflow.GetCommands(contextItem), contextItem);
                  }
                  if (CanShowCommands(contextItem, commandArray))
                  {
                    foreach (WorkflowCommand command in commandArray)
                    {
                      string displayName = command.DisplayName;
                      string str4 = new WorkflowCommandBuilder(contextItem, workflow, command).ToString();
                      if (Settings.WebEdit.AffectWorkflowForDatasourceItems)
                      {
                        str4 = str4.Replace("item:workflow(", "webedit:workflowwithdatasourceitems(");
                      }
                      PageEditorNotificationOption option = new PageEditorNotificationOption(displayName, str4);
                      item.Options.Add(option);
                    }
                  }
                  arguments.Notifications.Add(item);
                }
              }
            }
          }
        }
      }
    }
  }

}