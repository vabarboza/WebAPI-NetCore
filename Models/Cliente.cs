using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public string Endereco { get; set; }

        [Required]
        public string Telefone { get; set; }

        [Required]
        public string Celular { get; set; }

        [Required(ErrorMessage = "Email field is required.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string CpfCnpj { get; set; }

    }
}
