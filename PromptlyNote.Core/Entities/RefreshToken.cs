using System;
using System.Collections.Generic;
using System.Text;

namespace PromptlyNote.Core.Entities
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }

        // Сесія = один вхід (пристрій). Ротація зберігає SessionId, новий логін створює новий.
        public Guid SessionId { get; set; }

        public DateTime? RevokedAt { get; set; }
        public string? ReplacedByToken { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpiresAt;
    }
}
