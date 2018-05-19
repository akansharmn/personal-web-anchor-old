using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using VideoManagerService.Models;

namespace VideoManagerService.Controllers
{
    public class ChannelsController : ApiController
    {
        private VideoManagerServiceContext db = new VideoManagerServiceContext();

        // GET: api/Channels
        public IQueryable<Channel> GetChannels()
        {
            return db.Channels;
        }

        // GET: api/Channels/5
        [ResponseType(typeof(Channel))]
        public async Task<IHttpActionResult> GetChannel(int id)
        {
            Channel channel = await db.Channels.FindAsync(id);
            if (channel == null)
            {
                return NotFound();
            }

            return Ok(channel);
        }

        // PUT: api/Channels/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutChannel(int id, Channel channel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != channel.ChannelId)
            {
                return BadRequest();
            }

            db.Entry(channel).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChannelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Channels
        [ResponseType(typeof(Channel))]
        public async Task<IHttpActionResult> PostChannel(Channel channel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Channels.Add(channel);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = channel.ChannelId }, channel);
        }

        // DELETE: api/Channels/5
        [ResponseType(typeof(Channel))]
        public async Task<IHttpActionResult> DeleteChannel(int id)
        {
            Channel channel = await db.Channels.FindAsync(id);
            if (channel == null)
            {
                return NotFound();
            }

            db.Channels.Remove(channel);
            await db.SaveChangesAsync();

            return Ok(channel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ChannelExists(int id)
        {
            return db.Channels.Count(e => e.ChannelId == id) > 0;
        }
    }
}