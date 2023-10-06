using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Data.ToDo
{
    public enum ItemUpdateType
    {
        BotShutdown,
        StatusMessageAdded,

        UserAdded,
        UserRemoved,

        DescriptionUpdate,

        Completed,
        UnCompleted,

        CategoryAdded,
        CategoryRemoved,
        CategoryRenamed,

        StageAdded,
        StageRemoved,
        StageRenamed,

        TaskRemoved,
        ChannelMoved
    }

    public class ItemUpdate
    {
        public ItemUpdateType updateType;
        public string context;

        private static readonly Dictionary<ItemUpdateType, string> updateMessages = new()
        {
            { ItemUpdateType.BotShutdown, "The bot is down for maintenence, sorry for the inconvenience <3" },
            { ItemUpdateType.StatusMessageAdded, "Status message added {channelMention}" },

            { ItemUpdateType.UserAdded, "User {userMention} added to task" },
            { ItemUpdateType.UserRemoved, "User {userMention} removed from task" },

            { ItemUpdateType.DescriptionUpdate, "Task description updated by {userMention}" },

            { ItemUpdateType.Completed, "Task marked complete by {userMention}" },
            { ItemUpdateType.UnCompleted, "Task unmarked complete by {userMention}" },

            { ItemUpdateType.CategoryAdded, "Added category '{context}' to task" },
            { ItemUpdateType.CategoryRemoved, "Removed category '{context}' from task" },
            { ItemUpdateType.CategoryRenamed, "Category renamed: {renamedContext}" },

            { ItemUpdateType.StageAdded, "Added stage '{context}' to task" },
            { ItemUpdateType.StageRemoved, "Remove stage '{context}' from task'" },
            { ItemUpdateType.StageRenamed, "Stage renamed: {renamedContext}" },

            { ItemUpdateType.TaskRemoved, "Task removed by {userMention}\n\n**This post is archived and ignored by InnoTasker**" },
            { ItemUpdateType.ChannelMoved, "Post channel has been moved to {channelMention}" }
        };

        public ItemUpdate(ItemUpdateType updateType, string context)
        {
            this.updateType = updateType;
            this.context = context;
        }

        public override string ToString()
        {
            if (updateMessages.TryGetValue(updateType, out string message))
            {
                return ProcessMessage(message);
            }
            else
            {
                return $"NYI update type {updateType} with context {context}";
            }
        }

        public string ProcessMessage(string message)
        {
            message = message.Replace("{context}", context);
            message = message.Replace("{renamedContext}", string.Join(" --> ", context.Split(":")));
            if (!ulong.TryParse(context, out ulong temp)) return message;
            message = message.Replace("{userMention}", MentionUtils.MentionUser(ulong.Parse(context)));
            message = message.Replace("{channelMention}", MentionUtils.MentionChannel(ulong.Parse(context)));
            message = message.Replace("{roleMention}", MentionUtils.MentionRole(ulong.Parse(context)));
            return message;
        }
    }
}
