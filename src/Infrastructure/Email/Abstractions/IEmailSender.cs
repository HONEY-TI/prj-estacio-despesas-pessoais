using Domain.Entities;

namespace Infrastructure.Email.Abstractions;
public interface IEmailSender
{
    bool SendEmailPassword(Usuario usuario, string password);
}