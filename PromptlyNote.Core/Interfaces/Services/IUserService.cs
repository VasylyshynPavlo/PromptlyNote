using PromptlyNote.Core.DTOs.Forms;
using System;
using System.Collections.Generic;
using System.Text;

namespace PromptlyNote.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task CreateUser(CreateUserForm form, bool viaGoogle);
        Task DeleteUser(string id, string userId);
        Task ChangeFullName(string fullName, string userId);
    }
}
