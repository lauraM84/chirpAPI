using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chirpApi.Services.Model;
using chirpApi.Services.Services.ViewModel;

namespace chirpApi.Services.Services.Interfaces
{
    public interface IChirpsService
    {
        Task<List<ChirpViewModel>> GetChirpsByFilter(ChirpFilter filter);
    }
}
