using XTramile.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using XTramile.Infrastructure.Configuration;

namespace XTramile.Infrastructure.IServiceProvider
{
    public interface IXtramileCountryService
    {
        Task<IEnumerable<Country>> GetListCountry(string path);
        StatusFile StatusFileInfo {get;set;} // Actualy better in one service, cause share with CityService..but it is ok lah ya.
    }


}
