using System;
using System.Collections.Generic;

namespace MyEducation.MyDomain.Common
{

/// <summary>
/// Lớp cơ sở (Base Class) cho tất cả các Entities trong hệ thống.
/// Chứa các thuộc tính chung (Audit, Khóa chính) để tránh lặp lặp code.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Khóa chính (Primary Key). Đặt `protected set` để ngăn chặn gán ID tùy tiện từ bên ngoài.
    /// EF Core hoặc DB sẽ tự động gán giá trị khi SaveChanges.
    /// </summary>
    public int Id { get; protected set; }

    /// <summary>
    /// Dấu vết (Audit Trail): Thời gian tạo.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Tùy chọn: Người tạo (Lấy từ UserIdentity hoặc System).
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Dấu vết: Thời gian cập nhật sau cùng.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Tùy chọn: Người thực hiện cập nhật.
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Trạng thái xóa mềm (Soft Delete).
    /// Rất phổ biến ở các hệ thống để không bao giờ mất hẳn dữ liệu.
    /// </summary>
    public bool IsDeleted { get; protected set; } = false;

    // Các hàm helper hỗ trợ thay đổi trạng thái ngay tại Entity
    
    public void MarkAsDeleted(string deletedBy)
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = deletedBy;
    }

    public void MarkAsUpdated(string updatedBy)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    // ==========================================
    // CƠ CHẾ DOMAIN EVENTS (DDD)
    // ==========================================
    
    // Danh sách lưu trữ các sự kiện tạm thời trong bộ nhớ của Entity này
    private readonly List<BaseEvent> _domainEvents = new();

    /// <summary>
    /// Thuộc tính chỉ đọc (IReadOnlyCollection) để bên ngoài không dùng `Add` bậy bạ,
    /// mà phải gọi qua hàm `AddDomainEvent`.
    /// </summary>
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Đưa sự kiện vào danh sách hàng đợi của Entity.
    /// (Sẽ được EF Core Interceptor quét và bắn đi khi SaveChangesAsync).
    /// </summary>
    public void AddDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Xóa 1 sự kiện khỏi danh sách.
    /// </summary>
    public void RemoveDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    /// <summary>
    /// Xóa toàn bộ sự kiện sau khi đã Publish thành công.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
}