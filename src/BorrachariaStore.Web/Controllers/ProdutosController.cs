using BorrachariaStore.Web.Models;
using BorrachariaStore.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController(ProdutoService _produtoService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Produto>>> Listar()
    {
        var produtos = await _produtoService.ListarAsync();
        return Ok(produtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Produto>> ObterPorId(string id)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return BadRequest("ID de produto inválido.");
        }

        var produto = await _produtoService.ObterPorIdAsync(id);

        if (produto == null)
        {
            return NotFound("Produto não encontrado.");
        }

        return Ok(produto);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Produto>> Criar([FromBody] Produto produto)
    {
        if (produto.Aro <= 0)
        {
            return BadRequest("O aro do produto deve ser maior que zero.");
        }

        if (produto.Estoque < 0)
        {
            return BadRequest("A quantidade em estoque não pode ser negativa.");
        }

        produto.Id = null;

        await _produtoService.CriarAsync(produto);

        return CreatedAtAction(
            nameof(ObterPorId),
            new { id = produto.Id },
            produto);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(string id, [FromBody] Produto produto)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return BadRequest("ID de produto inválido.");
        }

        var produtoExistente = await _produtoService.ObterPorIdAsync(id);

        if (produtoExistente is null)
        {
            return NotFound("Produto não encontrado.");
        }

        if (produto.Aro <= 0)
        {
            return BadRequest("O aro do produto deve ser maior que zero.");
        }

        if (produto.Estoque < 0)
        {
            return BadRequest("A quantidade em estoque não pode ser negativa.");
        }

        await _produtoService.AtualizarAsync(id, produto);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Remover(string id)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return BadRequest("ID de produto inválido.");
        }

        var produtoExistente = await _produtoService.ObterPorIdAsync(id);

        if (produtoExistente is null)
        {
            return NotFound("Produto não encontrado.");
        }

        await _produtoService.RemoverAsync(id);
        return NoContent();
    }
}