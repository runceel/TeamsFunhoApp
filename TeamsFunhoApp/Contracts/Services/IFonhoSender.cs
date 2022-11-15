using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamsFunhoApp.Contracts.Services;
public interface IFunhoSender
{
    Task<(bool Success, string? Reason)> SendAsync(string message);
}
