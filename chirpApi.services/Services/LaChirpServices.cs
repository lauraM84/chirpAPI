using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chirpApi.Services.Model;
using chirpApi.Services.Model.DTOs;
using chirpApi.Services.Model.Filters;
using chirpApi.Services.Services.Interfaces;
using chirpApi.Services.ViewModel;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace chirpApi.Services.Services
{
    public class LaChirpServices : IChirpsService
    {
        private readonly CinguettioContext _context;

        public LaChirpServices(CinguettioContext context)
        {
            _context = context;
        }



        public async Task<List<ChirpViewModel>> GetChirpsByFilter(ChirpFilter filter)
        {
            IQueryable<Chirp> query = _context.Chirps.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Text))
            {
                query = query.Where(x => x.Text == filter.Text);
            }
            if (!string.IsNullOrWhiteSpace(filter.ExtUrl))
            {
                query = query.Where(x => x.ExtUrl.Contains(filter.ExtUrl));
            }

            var result = await query.Select(x => new ChirpViewModel
            {
                Id = x.Id,
                Text = x.Text,
                ExtUrl = x.ExtUrl,
                CreationTime = x.CreationTime,
                Lat = x.Lat,
                Lng = x.Lng,

            }).ToListAsync();


            return (result);

        }

        public async Task<List<ChirpViewModel>> GetAllChirps()
        {
            return await _context.Chirps.Select(x => new ChirpViewModel
            {
                Id = x.Id,
                Text = x.Text,
                ExtUrl = x.ExtUrl,
                CreationTime = x.CreationTime,
                Lat = x.Lat,
                Lng = x.Lng,

            }).ToListAsync();
        }

        public async Task<ChirpViewModel> GetChirpById(int id)
        {
            var entity = await _context.Chirps.FindAsync(id);

            if (entity == null)
            {
                return null; // or throw an exception, depending on your error handling strategy
            }

            return new ChirpViewModel
            {
                Id = entity.Id,
                Text = entity.Text,
                ExtUrl = entity.ExtUrl,
                CreationTime = entity.CreationTime,
                Lat = entity.Lat,
                Lng = entity.Lng
            };
        }

        public async Task<bool> UpdateChirp(int id, ChirpUpdateDTO chirp)
        {
            var entity = await _context.Chirps.FindAsync(id);

            if (entity == null)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(chirp.ExtUrl))
                entity.ExtUrl = chirp.ExtUrl;

            if (!string.IsNullOrWhiteSpace(chirp.Text))
                entity.Text = chirp.Text;

            if (chirp.Lng != null)
                entity.Lng = chirp.Lng;

            if (chirp.Lat != null)
                entity.Lat = chirp.Lat;

            _context.Entry(chirp).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;


        }

        public async Task<int?> CreateChirp(ChirpCreateDTO chirp)
        {
            if (string.IsNullOrWhiteSpace(chirp.Text))
            {
                return null; // or throw an exception, depending on your error handling strategy
            }
            var entity = new Chirp
            {
                Text = chirp.Text,
                ExtUrl = chirp.ExtUrl,
                Lat = chirp.Lat,
                Lng = chirp.Lng
            };
            _context.Chirps.Add(entity);
            await _context.SaveChangesAsync();

            return entity.Id;


        }

        public async Task<int?> DeleteChirp(int id)
        {   
            Chirp? entity = await _context.Chirps.Include(x => x.Comments)
                                                .Where(x => x.Id == id)
                                                .SingleOrDefaultAsync();

            if (entity == null)
            {
                return null; // or throw an exception, depending on your error handling strategy
            }

            if (entity.Comments != null || entity.Comments.Count > 0)
            {
                return -1;
            }

            _context.Chirps.Remove(entity);
            await _context.SaveChangesAsync();

            return id;
        }
    }
}
