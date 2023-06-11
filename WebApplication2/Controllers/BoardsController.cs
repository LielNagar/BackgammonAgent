using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardsController : ControllerBase
    {
        // GET: api/<BoardsController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<BoardsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<BoardsController>
        [HttpPost]
        //[Route("/api/Boards/{dice}")]
        public Board Post([FromBody] RequestModel requestModel)
        {
            Board board = requestModel.Board;
            Array.Sort(requestModel.Dice);
            int[] dice = requestModel.Dice;
            GameHandler gameHandler = new GameHandler();
            List<Board> possible = gameHandler.getPossibleBoards(board, dice);
            if (possible.Count == 0) return null;
            Board toReturn = possible[0];
            foreach(Board boardToGetHeuristic in possible)
            {
                boardToGetHeuristic.heuristic();
                if (toReturn.HeuristicScore < boardToGetHeuristic.HeuristicScore) toReturn = boardToGetHeuristic;
            }
             return toReturn;
        }   

        // PUT api/<BoardsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<BoardsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
