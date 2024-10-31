using Microsoft.AspNetCore.Mvc;
using buckeyebot.Domain.Catalog;
using buckeyebot.Data;

namespace buckeyebot.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly StoreContext _db;

        public CatalogController(StoreContext db)
        {
            _db = db;
        }

    [HttpGet]
    public IActionResult GetItems()
    {
        return Ok(_db.Items);
    }
    
[HttpGet("{id:int}")]
public IActionResult GetItem(int id)
{
     var item = _db.Items.Find(id);
     if (item == null) {
        return NotFound();
     }

    return Ok();
}

[HttpPost]
public IActionResult Post(Item item)
{
    _db.Items.Add(item);
    _db.SaveChanges();
    return Created($"/catalog/{item.Id}" , item);
}

[HttpPost("{id:int}/ratings")]
public IActionResult PostRating(int id, [FromBody] Rating rating)
{
    var item = _db.Items.Find(id);
    if (item == null) {
        return NotFound();
    }
    item.AddRating(rating);
    _db.SaveChanges();
    return Ok(item);
}
[HttpPut("{id;int}")]
public IActionResult Put(int id, Item item)
{
    return NoContent();
}
    
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        return NoContent();
    }
}

}

