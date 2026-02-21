using Microsoft.AspNetCore.Mvc;
using WebApiP33.Models;

namespace WebApiP33.Controllers;

[ApiController]
[Route("[controller]")] // /calculator
public class CalculatorController : ControllerBase
{


    [HttpPost("sum")] // /calculator/sum
    public CalculatorResult Sum(CalculatorOperation operation)
    {
        return new CalculatorResult
        {
            Result = operation.Number1 + operation.Number2
        };
    }

    [HttpPost("subtract")] // /calculator/subtract
    public CalculatorResult Subtract(CalculatorOperation operation)
    {
        return new CalculatorResult
        {
            Result = operation.Number1 - operation.Number2
        };
    }

    [HttpPost("sum-many")]
    public IEnumerable<CalculatorResult> Sum(List<CalculatorOperation> operations)
    {
        return operations.Select(x => new CalculatorResult { Result = x.Number1 + x.Number2 });
    }
}
