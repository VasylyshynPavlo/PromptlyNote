namespace PromptlyNote.Core.Entities
{
    public class GoogleCalendarConnection : BaseEntity
    {
        public Guid UserId { get; set; }
        public User? User { get; set; }

        public string EncryptedRefreshToken { get; set; } = string.Empty;
        public string Scopes { get; set; } = string.Empty;
    }
}
