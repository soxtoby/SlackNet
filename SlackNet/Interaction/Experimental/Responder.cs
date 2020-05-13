using System.Threading.Tasks;

namespace SlackNet.Interaction.Experimental
{
    public delegate Task Responder();
    public delegate Task Responder<in T>(T response);
}