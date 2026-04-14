using System;
using System.Collections.Generic;


namespace MyEducation.MyDomain.Common
{   

/// <summary>
/// Lớp cơ sở cho mọi Sự kiện Nghiệp vụ (Domain Event).
/// Các Event này mang tính quá khứ (đã xảy ra rồi), ví dụ: CoursePublishedEvent, ExamSubmittedEvent.
/// </summary>
public abstract class BaseEvent
{
    /// <summary>
    /// Thời điểm sự kiện này được hệ thống ghi nhận.
    /// </summary>
    public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;

    /// <summary>
    /// Tùy chọn: Có đang trong trạng thái chờ xử lý ở các Queue/Background hay không.
    /// </summary>
    public bool IsPublished { get; set; } = false;
}
}