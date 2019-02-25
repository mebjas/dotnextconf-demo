namespace dotnextconf_demo.Azure
{
    using System.Threading.Tasks;

    public interface IPublisher
    {
        Task PublishAsync(string message);
    }
}
