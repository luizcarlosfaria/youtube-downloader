using System;
using System.Collections.Generic;
using System.Linq;

namespace DevWeek.Architecture.Services.SpringServices;

/// <summary>
/// Serviço de startup automático de serviços
/// </summary>
public class GenericStartUpService : IService
{
    /// <summary>
    /// Nome do Serviço
    /// </summary>
    public string Name
    {
        get { return "GenericStartUpService"; }
    }

    /// <summary>
    /// Objeto responsável pelo startup EntryPoint
    /// </summary>
    public object HandlerObject { get; set; }

    /// <summary>
    /// Objeto responsável pelo startup EntryPoint
    /// </summary>
    public List<object> HandlerObjects { get; set; }

    /// <summary>
    /// Método (void) a ser chamado durante a inicialização
    /// </summary>
    public string StartMethodName { get; set; }

    /// <summary>
    /// Método (void) a ser chamado durante a inicialização
    /// </summary>
    public string StopMethodName { get; set; }

    /// <summary>
    /// Responde pela inicialização do processamento
    /// </summary>
    public void Start()
    {
        if (string.IsNullOrWhiteSpace(this.StartMethodName) == false)
            this.CallMethod(this.StartMethodName, Order.Desc);
    }

    /// <summary>
    /// Identifica o que fazer ao terminar o processamento
    /// </summary>
    public void Stop()
    {
        if (string.IsNullOrWhiteSpace(this.StopMethodName) == false)
            this.CallMethod(this.StopMethodName, Order.Desc);
    }

    /// <summary>
    /// Executa a chamada ao método especificado
    /// </summary>
    /// <param name="methodName"></param>
    private void CallMethod(string methodName, Order orderToExecute)
    {
        if (this.HandlerObjects == null)
            this.HandlerObjects = new List<object>();

        if (this.HandlerObject != null && this.HandlerObjects.Contains(this.HandlerObject) == false)
            this.HandlerObjects.Insert(0, this.HandlerObject);

        if (this.HandlerObjects.Count == 0)
            throw new InvalidOperationException("Não há itens configurados para a execução. Use as propriedades HandlerObject ou HandlerObjects");

        IEnumerable<object> listToExecute = orderToExecute == Order.Asc ? this.HandlerObjects : new List<object>(this.HandlerObjects).Reverse<object>();

        foreach (var currentObject in listToExecute)
        {
            Spring.Expressions.ExpressionEvaluator.GetValue(currentObject, methodName + "()", null);
        }
    }

    private enum Order
    {
        Asc,
        Desc
    }
}