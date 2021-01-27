using ApiCore.Business.Notifications;
using System.Collections.Generic;

namespace ApiCore.Business.Intefaces
{
    public interface INotificador
    {
        bool TemNotificacao();

        List<Notificacao> ObterNotificacoes();

        void Handle(Notificacao notificacao);
    }
}