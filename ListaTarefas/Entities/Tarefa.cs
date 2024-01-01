namespace ListaTarefas.Entities;
using System;

public class Tarefa
{
    public Tarefa(string titulo, string descricao, bool status)
    {
        Titulo = titulo;
        Descricao = descricao;
        Status = status;
    }
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Titulo { get; set; }
    public string ?Descricao { get; set; }
    public bool Status { get; set; }
    public void Update(string titulo, string descricao ){
            Titulo = titulo;
            Descricao = descricao;
        }

}
