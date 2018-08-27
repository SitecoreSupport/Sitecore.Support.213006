namespace Sitecore.Support.Pipelines.GetPageEditorNotifications
{
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.ExperienceEditor;
  using Sitecore.ExperienceEditor.Switchers;
  using Sitecore.ExperienceEditor.Utils;
  using Sitecore.Globalization;
  using Sitecore.SecurityModel;
  using Sitecore.Shell.Applications.ContentManager.Panels;
  using Sitecore.Shell.Framework.CommandBuilders;
  using Sitecore.Workflows;
  using Sitecore.Pipelines.GetPageEditorNotifications;

  public class GetWorkflowNotification : GetPageEditorNotificationsProcessor
  {
    private static string GetDescription(IWorkflow workflow, WorkflowState state, Database database)
    {
      return WorkflowUtility.GetWorkflowStateDescription(workflow, state, database);
    }

    public override void Process(GetPageEditorNotificationsArgs arguments)
    {
      Assert.ArgumentNotNull(arguments, "arguments");
      if (!WebUtility.IsEditAllVersionsTicked())
      {
        Item contextItem = arguments.ContextItem;
        if ((contextItem == null) || ((contextItem.Access.CanReadLanguage() && contextItem.Access.CanWriteLanguage()) && contextItem.Access.CanWrite()))
        {
          using (new SecurityDisabler())
          {
            Database database = (contextItem != null) ? contextItem.Database : null;
            IWorkflow workflow = (((database != null) ? database.WorkflowProvider : null) == null) ? null : ((database != null) ? database.WorkflowProvider : null).GetWorkflow(contextItem);
            WorkflowState state = (workflow != null) ? workflow.GetState(contextItem) : null;
            if (state != null)
            {
              using (new LanguageSwitcher(WebUtility.ClientLanguage))
              {
                WorkflowCommand[] commandArray;
                string icon = state.Icon;
                PageEditorNotification item = new PageEditorNotification(GetDescription(workflow, state, database), PageEditorNotificationType.Information)
                {
                  Icon = icon
                };
                using (new SecurityEnabler())
                {
                  using (new ClientDatabaseSwitcher(database))
                  {
                    commandArray = WorkflowFilterer.FilterVisibleCommands(workflow.GetCommands(contextItem), contextItem);
                  }
                }

                if (WorkflowPanel.CanShowCommands(contextItem, commandArray))
                {
                  foreach (WorkflowCommand command in commandArray)
                  {
                    string str2 = new WorkflowCommandBuilder(contextItem, workflow, command).ToString();
                    if (Settings.WebEdit.AffectWorkflowForDatasourceItems)
                    {
                      str2 = str2.Replace("item:workflow(", "webedit:workflowwithdatasourceitems(");
                    }
                    PageEditorNotificationOption option = new PageEditorNotificationOption(command.DisplayName, str2);
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