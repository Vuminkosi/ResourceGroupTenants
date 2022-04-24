namespace ResourceGroupTenants.Core.Models.Response
{
    public class ApiRequest<T>
    {
        public ApiRequest()
        {

        }

        public ApiRequest(T request) : this()
        {
            Request = request;
        }
        public T Request { get; set; }
    }


}
