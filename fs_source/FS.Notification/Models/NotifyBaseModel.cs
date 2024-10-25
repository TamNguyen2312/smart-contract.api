using System;

namespace FS.Notification.Models;

public class NotifyBaseModel
{
    /// <summary>
    /// The actor's name
    /// </summary>
    public string ActorName { get; set; }
    /// <summary>
    /// The actor's url that redirect to actor detail page.
    /// </summary>
    public string ActorHref { get; set; }
    /// <summary>
    /// The left icon name, using icons.css
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// The created time of notification.
    /// </summary>
    public DateTime CreatedTime { get; set; }
}