using System;
using System.Text;
using FS.Commons;
using FS.Notification.Models;

namespace FS.Notification;

public class NotificationBuilder
{
    private readonly static object lockObject = new object();
    private static NotificationBuilder _builder;
    public static NotificationBuilder GetInstance()
    {
        lock (lockObject)
        {
            if (_builder == null)
                _builder = new NotificationBuilder();
            return _builder;
        }
    }

    private string DateTimeAgo(DateTime input)
    {
        var time = DateTime.Now.Subtract(input);
        int hours = time.Hours;
        int minute = time.Minutes;
        if (minute <= 1)
            return "Vừa xong";
        if (hours < 1)
            return string.Format("{0} phút trước", minute);
        if (hours < 24)
            return string.Format("{0} giờ trước", hours);
        if (hours < 24 * 7)
            return string.Format("{0} ngày trước", time.Days);
        return time.ToString(Constants.FormatDateTime);
    }

    public string BuildContent<T>(NotifyType notifyType, T model) where T : NotifyBaseModel
    {
        StringBuilder builder = new StringBuilder();
        builder.Append($"<a href=\"{model.ActorHref}\" class=\"dropdown-item notify-item\">");
        if (!string.IsNullOrEmpty(model.Icon))
        {
            builder.Append($"<div class=\"notify-icon bg-success\"><i class=\"{model.Icon}\"></i></div>");
        }
        string content = string.Empty;
        switch (notifyType)
        {
            case NotifyType.SellerSubmitDepositForm:
                string text = string.Format(NotifyContentConstants.SELLER_SUBMIT_DEPOSIT_FORM, model.ActorName);
                content = BuildNotificationContent(text, model.CreatedTime);
                break;
            case NotifyType.Logout:
                break;
            default:
                break;
        }
        builder.Append(content);
        builder.Append("</a>");
        return builder.ToString();
    }

    private string BuildNotificationContent(string text, DateTime createdTime)
    {
        return $"<p class=\"notify-details\">{text}<small class=\"text-muted\">{DateTimeAgo(createdTime)}</small></p>";
    }
}