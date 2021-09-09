using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Persistence;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlickListBaseController : ControllerBase
    {
        protected readonly DataContext _context;
        protected readonly IMapper _mapper;
        private DataContext context;

        public FlickListBaseController(DataContext context)
        {
            this.context = context;
        }

        public FlickListBaseController(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }
    }
}