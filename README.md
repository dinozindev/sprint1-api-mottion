# Mottu Mottion - API

## Descrição do Projeto

A Mottion nasce como resposta a um dos maiores gargalos operacionais
enfrentados atualmente pela Mottu: a falta de controle, rastreabilidade e padronização
na gestão de pátios de motos em suas filiais. Com mais de 100 unidades espalhadas
por diferentes regiões e layouts, a tarefa de localizar motos, controlar manutenções e
evitar furtos se tornou complexa, sujeita a falhas manuais e impactos diretos na
produtividade e segurança. 

Nosso projeto tem como missão automatizar e otimizar a operação física de
motos no pátio, garantindo visibilidade em tempo real, ações corretivas proativas e
total rastreabilidade — usando sensores IoT, visão computacional e um sistema web
/ mobile responsivo.

A API construída em .NET é responsável por gerenciar informações sobre os clientes, motos, pátios, vagas, setores, funcionários, gerentes, cargos e movimentações. Além disso, ela será de suma importância para a atualização e localização em tempo real da ocupação de motos em cada um dos setores do pátio, além de fornecer o histórico de movimentações de uma moto dentro da filial.

## Instalação

### Instalação e Execução da API (.NET 9)
#### 📋 Pré-requisitos
Antes de instalar, verifique se os seguintes itens estão instalados:

- .NET 9 SDK

- Oracle Database ou acesso a um banco Oracle

- Oracle Entity Framework Core Provider

- Visual Studio 2022+ ou Rider (opcional)

- Git (opcional)

### Clone o repositório e acesse o diretório:

```bash
git clone https://github.com/dinozindev/sprint1-api-mottion.git
cd sprint1-api-mottion
cd Sprint1-API
```

### Instale as dependências:
```bash
dotnet restore
```

### Se deseja utilizar o banco de dados Oracle já desenvolvido (com todos os inserts), não altere nada. Caso queira criar, altere o appsettings.json com suas credenciais:
```code
"ConnectionStrings": {
    "OracleConnection": "User Id=<seuid>;Password=<suasenha>;Data Source=<source>;"
  }
```

### E execute para criar as tabelas: 
```bash
dotnet ef database update
```

### Inicie a aplicação: 
```bash
dotnet run
```

### Para acessar a documentação da aplicação: 
```bash
http://localhost:5147/scalar
```

### Se deseja utilizar a API localmente, em Program.cs mantenha a seguinte linha comentada:
```code
//builder.WebHost.UseUrls("http://0.0.0.0:5147");
```

## Rotas da API

### Clientes

- #### Retorna todos os clientes

```http
  GET /clientes
```
Response Body:

```json
[
  {
    "clienteId": 1,
    "nomeCliente": "string",
    "telefoneCliente": "string",
    "sexoCliente": "string",
    "emailCliente": "string",
    "cpfCliente": "string",
    "motos": [
      {
        "motoId": 1,
        "placaMoto": null,
        "modeloMoto": "string",
        "situacaoMoto": "string",
        "chassiMoto": "string"
      }
    ]
  }
]
```

Códigos de Resposta

| Código HTTP | Significado                     | Quando ocorre                                             |
|-------------|----------------------------------|-----------------------------------------------------------|
| 200 OK      | Requisição bem-sucedida         | Quando há clientes cadastrados                            |
| 204 No Content | Sem conteúdo a retornar      | Quando não há clientes cadastrados                        |
| 500 Internal Server Error | Erro interno     | Quando ocorre uma falha inesperada no servidor            |

- #### Retorna um cliente pelo ID

```http
  GET /clientes/{id}
```

| Parâmetro   | Tipo       | Descrição                                   |
| :---------- | :--------- | :------------------------------------------ |
| `id`      | `int` | **Obrigatório**. O ID do cliente que você deseja consultar |

Response Body:

```json
  {
    "clienteId": 1,
    "nomeCliente": "string",
    "telefoneCliente": "string",
    "sexoCliente": "string",
    "emailCliente": "string",
    "cpfCliente": "string",
    "motos": [
      {
        "motoId": 1,
        "placaMoto": null,
        "modeloMoto": "string",
        "situacaoMoto": "string",
        "chassiMoto": "string"
      }
    ]
  }
```

Códigos de Resposta

| Código HTTP | Significado                     | Quando ocorre                                             |
|-------------|----------------------------------|-----------------------------------------------------------|
| 200 OK      | Requisição bem-sucedida         | Quando o cliente foi encontrado                            |
| 404 Not Found | Recurso não encontrado        | Quando o cliente especificado não existe       |
| 500 Internal Server Error | Erro interno     | Quando ocorre uma falha inesperada no servidor            |

### Motos

- #### Retorna todas as motos

```http
  GET /motos
```

Response Body:

```json
[
  {
    "motoId": 1,
    "placaMoto": "string",
    "modeloMoto": "string",
    "situacaoMoto": "string",
    "chassiMoto": "string",
    "cliente": {
      "clienteId": 1,
      "nomeCliente": "string",
      "telefoneCliente": "string",
      "sexoCliente": "string",
      "emailCliente": "string",
      "cpfCliente": "string"
    }
  }
]
```

Códigos de Resposta

| Código HTTP | Significado                     | Quando ocorre                                             |
|-------------|----------------------------------|-----------------------------------------------------------|
| 200 OK      | Requisição bem-sucedida         | Quando há motos cadastradas                            |
| 204 No Content | Sem conteúdo a retornar      | Quando não há motos cadastradas                        |
| 500 Internal Server Error | Erro interno     | Quando ocorre uma falha inesperada no servidor            |

- #### Retorna uma moto pelo ID

```http
  GET /motos/{id}
```

| Parâmetro   | Tipo       | Descrição                                   |
| :---------- | :--------- | :------------------------------------------ |
| `id`      | `int` | **Obrigatório**. O ID da moto que você deseja consultar |

```json
  {
    "motoId": 1,
    "placaMoto": "string",
    "modeloMoto": "string",
    "situacaoMoto": "string",
    "chassiMoto": "string",
    "cliente": {
      "clienteId": 1,
      "nomeCliente": "string",
      "telefoneCliente": "string",
      "sexoCliente": "string",
      "emailCliente": "string",
      "cpfCliente": "string"
    }
  }
```
Códigos de Resposta

| Código HTTP | Significado                     | Quando ocorre                                             |
|-------------|----------------------------------|-----------------------------------------------------------|
| 200 OK      | Requisição bem-sucedida         | Quando a moto foi encontrada                            |
| 404 Not Found | Recurso não encontrado        | Quando a moto especificada não existe       |
| 500 Internal Server Error | Erro interno     | Quando ocorre uma falha inesperada no servidor            |

- #### Retorna uma moto pelo ID

```http
  GET /motos/por-chassi/{numero-chassi}
```

| Parâmetro   | Tipo       | Descrição                                   |
| :---------- | :--------- | :------------------------------------------ |
| `numero-chassi`      | `string` | **Obrigatório**. O número de chassi da moto que você deseja consultar |

```json
  {
    "motoId": 1,
    "placaMoto": "string",
    "modeloMoto": "string",
    "situacaoMoto": "string",
    "chassiMoto": "string",
    "cliente": {
      "clienteId": 1,
      "nomeCliente": "string",
      "telefoneCliente": "string",
      "sexoCliente": "string",
      "emailCliente": "string",
      "cpfCliente": "string"
    }
  }
```
Códigos de Resposta

| Código HTTP | Significado                     | Quando ocorre                                             |
|-------------|----------------------------------|-----------------------------------------------------------|
| 200 OK      | Requisição bem-sucedida         | Quando a moto foi encontrada                            |
| 404 Not Found | Recurso não encontrado        | Quando a moto especificada não existe       |
| 500 Internal Server Error | Erro interno     | Quando ocorre uma falha inesperada no 


- #### Retorna a última posição de uma moto pelo ID

```http
  GET /motos/{id}/ultima-posicao
```

| Parâmetro   | Tipo       | Descrição                                   |
| :---------- | :--------- | :------------------------------------------ |
| `id`      | `int` | **Obrigatório**. O ID da moto que você deseja obter a última posição |

Response Body: 

```json
{
  "vaga": {
    "vagaId": 1,
    "numeroVaga": "string",
    "statusOcupada": 1
  },
  "setor": {
    "setorId": 1,
    "tipoSetor": "string",
    "statusSetor": "string",
    "patioId": 1
  },
  "dtEntrada": "2025-05-08T18:51:02.420Z",
  "dtSaida": null,
  "permanece": true
}
```

Códigos de Resposta

| Código HTTP | Significado                     | Quando ocorre                                             |
|-------------|----------------------------------|-----------------------------------------------------------|
| 200 OK      | Requisição bem-sucedida         | Quando a posição anterior é encontrada                             |
| 404 Not Found | Recurso não encontrado        |  Quando nenhuma posição anterior foi encontrada       |
| 500 Internal Server Error | Erro interno     | Quando ocorre uma falha inesperada no servidor            |

- #### Cria uma moto

```http
  POST /motos
```

Request Body:

```json
{
  "placaMoto": null,
  "modeloMoto": "",
  "situacaoMoto": "",
  "chassiMoto": ""
}
```

Códigos de Resposta

| Código HTTP       | Significado                     | Quando ocorre                                                  |
|-------------------|----------------------------------|----------------------------------------------------------------|
| 201 Created       | Recurso criado com sucesso      | Quando uma moto é criada com êxito |
| 400 Bad Request   | Requisição malformada           | Quando os dados enviados estão incorretos ou incompletos       |
| 409 Conflict      | Conflito de estado              | Quando há conflito, como dados duplicados (placa e chassi)                     |
| 500 Internal Server Error | Erro interno             | Quando ocorre uma falha inesperada no servidor                 |

- #### Atualiza uma moto

```http
  PUT /motos/{id}
```

Request Body:

```json
{
  "placaMoto": null,
  "modeloMoto": "",
  "situacaoMoto": "",
  "chassiMoto": ""
}
```

| Parâmetro   | Tipo       | Descrição                                   |
| :---------- | :--------- | :------------------------------------------ |
| `id`      | `int` | **Obrigatório**. O ID da moto que você atualizar |

Códigos de Resposta

| Código HTTP       | Significado                     | Quando ocorre                                                  |
|-------------------|----------------------------------|----------------------------------------------------------------|
| 201 Created       | Recurso criado com sucesso      | Quando uma moto é criada com êxito |
| 400 Bad Request   | Requisição malformada           | Quando os dados enviados estão incorretos ou incompletos       |
| 404 Not Found | Recurso não encontrado        |  Quando nenhuma moto foi encontrada com o ID especificado      |
| 409 Conflict      | Conflito de estado              | Quando há conflito, como dados duplicados (placa e chassi)                     |
| 500 Internal Server Error | Erro interno             | Quando ocorre uma falha inesperada no servidor                 |



- #### Remove um cliente da moto

```http
  PUT /motos/{id}/remover-cliente
```

| Parâmetro   | Tipo       | Descrição                                   |
| :---------- | :--------- | :------------------------------------------ |
| `id`      | `int` | **Obrigatório**. O ID da moto que você atualizar |
| `clienteId`      | `int` | **Obrigatório**. O ID do cliente que você deseja remover |

Códigos de Resposta

| Código HTTP       | Significado                     | Quando ocorre                                                  |
|-------------------|----------------------------------|----------------------------------------------------------------|
| 204 No Content    | Sem conteúdo a retornar         | Quando a atualização da moto é válida, mas não há dados para retornar   |
| 404 Not Found     | Recurso não encontrado          | Quando a moto não é encontrada                 |
| 500 Internal Server Error | Erro interno             | Quando ocorre uma falha inesperada no servidor                 |


- #### Altera / Adiciona um cliente a moto

```http
  PUT /motos/{id}/alterar-cliente/{clienteId}
```

| Parâmetro   | Tipo       | Descrição                                   |
| :---------- | :--------- | :------------------------------------------ |
| `id`      | `int` | **Obrigatório**. O ID da moto que você atualizar |
| `clienteId`      | `int` | **Obrigatório**. O ID do cliente que você deseja atualizar / adicionar |

Códigos de Resposta

| Código HTTP       | Significado                     | Quando ocorre                                                  |
|-------------------|----------------------------------|----------------------------------------------------------------|
| 204 No Content    | Sem conteúdo a retornar         | Quando a atualização da moto é válida, mas não há dados para retornar   |
| 404 Not Found     | Recurso não encontrado          | Quando a moto não é encontrada                 |
| 500 Internal Server Error | Erro interno             | Quando ocorre uma falha inesperada no servidor                 |

- #### Deleta uma moto

```http
  DELETE /motos/{id}
```

| Parâmetro   | Tipo       | Descrição                                   |
| :---------- | :--------- | :------------------------------------------ |
| `id`      | `int` | **Obrigatório**. O ID da moto que você deseja deletar |

Códigos de Resposta

| Código HTTP       | Significado                     | Quando ocorre                                                  |
|-------------------|----------------------------------|----------------------------------------------------------------|
| 204 No Content    | Sem conteúdo a retornar         | Quando a remoção da moto é válida, mas não há dados para retornar   |
| 404 Not Found     | Recurso não encontrado          | Quando a moto especificada não é encontrada                |
| 500 Internal Server Error | Erro interno             | Quando ocorre uma falha inesperada no servidor                 |


### Patios

- #### Retorna a lista de pátios com setores e vagas

```http
  GET /patios
```

Response Body: 

```json
[
  {
    "patioId": 1,
    "localizacaoPatio": "string",
    "nomePatio": "string",
    "descricaoPatio": "string",
    "setores": [
      {
        "setorId": 1,
        "tipoSetor": "string",
        "statusSetor": "string",
        "vagas": [
          {
            "vagaId": 1,
            "numeroVaga": "string",
            "statusOcupada": 1
          }
        ]
      }
    ]
  }
]
```

Códigos de Resposta

| Código HTTP       | Significado                     | Quando ocorre                                                  |
|-------------------|----------------------------------|----------------------------------------------------------------|
| 200 OK            | Requisição bem-sucedida         | Quando os patios são encontrados                     |
| 204 No Content    | Sem conteúdo a retornar         | Quando nenhum pátio existe  |
| 500 Internal Server Error | Erro interno             | Quando ocorre uma falha inesperada no servidor                 |

- #### Retorna um pátio a partir de um ID

```http
  GET /patios/{id}
```

| Parâmetro   | Tipo       | Descrição                                   |
| :---------- | :--------- | :------------------------------------------ |
| `id`      | `int` | **Obrigatório**. O ID do pátio que deseja consultar |

Response Body: 

```json
  {
    "patioId": 1,
    "localizacaoPatio": "string",
    "nomePatio": "string",
    "descricaoPatio": "string",
    "setores": [
      {
        "setorId": 1,
        "tipoSetor": "string",
        "statusSetor": "string",
        "vagas": [
          {
            "vagaId": 1,
            "numeroVaga": "string",
            "statusOcupada": 1
          }
        ]
      }
    ]
  }
```

Códigos de Resposta

| Código HTTP       | Significado                     | Quando ocorre                                                  |
|-------------------|----------------------------------|----------------------------------------------------------------|
| 200 OK            | Requisição bem-sucedida         | Quando o pátio é encontrado                     |
| 404 Not Found     | Recurso não encontrado          | Quando o pátio especificado não é encontrado               |
| 500 Internal Server Error | Erro interno             | Quando ocorre uma falha inesperada no servidor                 |

### Cargos

- #### Retorna a lista de cargos

```http
  GET /cargos
```

Response Body: 

```json
[
  {
    "cargoId": 1,
    "nomeCargo": "string",
    "descricaoCargo": "string"
  }
]
```

Códigos de Resposta

| Código HTTP       | Significado                     | Quando ocorre                                                  |
|-------------------|----------------------------------|----------------------------------------------------------------|
| 200 OK            | Requisição bem-sucedida         | Quando os cargos são encontrados                     |
| 204 No Content    | Sem conteúdo a retornar         | Quando nenhum cargo existe  |
| 500 Internal Server Error | Erro interno             | Quando ocorre uma falha inesperada no servidor                 |

- #### Retorna um cargo a partir de um ID

```http
  GET /cargos/{id}
```

| Parâmetro   | Tipo       | Descrição                                   |
| :---------- | :--------- | :------------------------------------------ |
| `id`      | `int` | **Obrigatório**. O ID do cargo que deseja consultar |

Response Body: 

```json
  {
    "cargoId": 1,
    "nomeCargo": "string",
    "descricaoCargo": "string"
  }
```

Códigos de Resposta

| Código HTTP       | Significado                     | Quando ocorre                                                  |
|-------------------|----------------------------------|----------------------------------------------------------------|
| 200 OK            | Requisição bem-sucedida         | Quando o cargo é encontrado                     |
| 404 Not Found     | Recurso não encontrado          | Quando o cargo especificado não é encontrado               |
| 500 Internal Server Error | Erro interno             | Quando ocorre uma falha inesperada no servidor                 |

### Vagas

- #### Retorna a lista de vagas

```http
  GET /vagas
```

Response Body: 

```json
[
  {
    "vagaId": 1,
    "numeroVaga": "string",
    "statusOcupada": 1,
    "setor": {
      "setorId": 1,
      "tipoSetor": "string",
      "statusSetor": "string",
      "patioId": 1
    }
  }
]
```

Códigos de Resposta

| Código HTTP       | Significado                     | Quando ocorre                                                  |
|-------------------|----------------------------------|----------------------------------------------------------------|
| 200 OK            | Requisição bem-sucedida         | Quando as vagas são encontradas                     |
| 204 No Content    | Sem conteúdo a retornar         | Quando nenhuma vaga existe  |
| 500 Internal Server Error | Erro interno             | Quando ocorre uma falha inesperada no servidor                 |

- #### Retorna uma vaga a partir de um ID

```http
  GET /vagas/{id}
```

| Parâmetro   | Tipo       | Descrição                                   |
| :---------- | :--------- | :------------------------------------------ |
| `id`      | `int` | **Obrigatório**. O ID da vaga que deseja consultar |

Response Body: 

```json
  {
    "vagaId": 1,
    "numeroVaga": "string",
    "statusOcupada": 1,
    "setor": {
      "setorId": 1,
      "tipoSetor": "string",
      "statusSetor": "string",
      "patioId": 1
    }
  }
```

Códigos de Resposta

| Código HTTP       | Significado                     | Quando ocorre                                                  |
|-------------------|----------------------------------|----------------------------------------------------------------|
| 200 OK            | Requisição bem-sucedida         | Quando a vaga é encontrada                     |
| 404 Not Found     | Recurso não encontrado          | Quando a vaga especificada não é encontrada               |
| 500 Internal Server Error | Erro interno             | Quando ocorre uma falha inesperada no servidor                 |

### Funcionarios

- #### Retorna a lista de funcionários

```http
  GET /funcionarios
```

Response Body: 

```json
[
  {
    "funcionarioId": 1,
    "nomeFuncionario": "string",
    "telefoneFuncionario": "string",
    "cargo": {
      "cargoId": 1,
      "nomeCargo": "string",
      "descricaoCargo": "string"
    },
    "patio": {
      "patioId": 1,
      "localizacaoPatio": "string",
      "nomePatio": "string",
      "descricaoPatio": "string"
    }
  }
]
```

Códigos de Resposta

| Código HTTP       | Significado                     | Quando ocorre                                                  |
|-------------------|----------------------------------|----------------------------------------------------------------|
| 200 OK            | Requisição bem-sucedida         | Quando os funcionários são encontrados                     |
| 204 No Content    | Sem conteúdo a retornar         | Quando nenhum funcionário existe  |
| 500 Internal Server Error | Erro interno             | Quando ocorre uma falha inesperada no servidor                 |

- #### Retorna um funcionário a partir de um ID

```http
  GET /funcionarios/{id}
```

| Parâmetro   | Tipo       | Descrição                                   |
| :---------- | :--------- | :------------------------------------------ |
| `id`      | `int` | **Obrigatório**. O ID do funcionário que deseja consultar |

Response Body: 

```json
  {
    "funcionarioId": 1,
    "nomeFuncionario": "string",
    "telefoneFuncionario": "string",
    "cargo": {
      "cargoId": 1,
      "nomeCargo": "string",
      "descricaoCargo": "string"
    },
    "patio": {
      "patioId": 1,
      "localizacaoPatio": "string",
      "nomePatio": "string",
      "descricaoPatio": "string"
    }
  }
```

Códigos de Resposta

| Código HTTP       | Significado                     | Quando ocorre                                                  |
|-------------------|----------------------------------|----------------------------------------------------------------|
| 200 OK            | Requisição bem-sucedida         | Quando o funcionário é encontrado                     |
| 404 Not Found     | Recurso não encontrado          | Quando o funcionário especificado não é encontrado               |
| 500 Internal Server Error | Erro interno             | Quando ocorre uma falha inesperada no servidor                 |

### Gerentes

- #### Retorna a lista de gerentes

```http
  GET /gerentes
```

Response Body: 

```json
[
  {
    "gerenteId": 1,
    "nomeGerente": "string",
    "telefoneGerente": "string",
    "cpfGerente": "string",
    "patio": {
      "patioId": 1,
      "localizacaoPatio": "string",
      "nomePatio": "string",
      "descricaoPatio": "string"
    }
  }
]
```

Códigos de Resposta

| Código HTTP       | Significado                     | Quando ocorre                                                  |
|-------------------|----------------------------------|----------------------------------------------------------------|
| 200 OK            | Requisição bem-sucedida         | Quando os gerentes são encontrados                     |
| 204 No Content    | Sem conteúdo a retornar         | Quando nenhum gerente existe  |
| 500 Internal Server Error | Erro interno             | Quando ocorre uma falha inesperada no servidor                 |

- #### Retorna um gerente a partir de um ID

```http
  GET /gerentes/{id}
```

| Parâmetro   | Tipo       | Descrição                                   |
| :---------- | :--------- | :------------------------------------------ |
| `id`      | `int` | **Obrigatório**. O ID do gerente que deseja consultar |

Response Body: 

```json
  {
    "gerenteId": 1,
    "nomeGerente": "string",
    "telefoneGerente": "string",
    "cpfGerente": "string",
    "patio": {
      "patioId": 1,
      "localizacaoPatio": "string",
      "nomePatio": "string",
      "descricaoPatio": "string"
    }
  }
```

Códigos de Resposta

| Código HTTP       | Significado                     | Quando ocorre                                                  |
|-------------------|----------------------------------|----------------------------------------------------------------|
| 200 OK            | Requisição bem-sucedida         | Quando o gerente é encontrado                     |
| 404 Not Found     | Recurso não encontrado          | Quando o gerente especificado não é encontrado               |
| 500 Internal Server Error | Erro interno             | Quando ocorre uma falha inesperada no servidor                 |

### Setores

- #### Retorna a lista de setores

```http
  GET /setores
```

Response Body: 

```json
[
  {
    "setorId": 1,
    "tipoSetor": "string",
    "statusSetor": "string",
    "patio": {
      "patioId": 1,
      "localizacaoPatio": "string",
      "nomePatio": "string",
      "descricaoPatio": "string"
    },
    "vagas": [
      {
        "vagaId": 1,
        "numeroVaga": "string",
        "statusOcupada": 1
      }
    ]
  }
]
```

Códigos de Resposta

| Código HTTP       | Significado                     | Quando ocorre                                                  |
|-------------------|----------------------------------|----------------------------------------------------------------|
| 200 OK            | Requisição bem-sucedida         | Quando os setores são encontrados                     |
| 204 No Content    | Sem conteúdo a retornar         | Quando nenhum setor existe  |
| 500 Internal Server Error | Erro interno             | Quando ocorre uma falha inesperada no servidor                 |


- #### Retorna um setor a partir de um ID

```http
  GET /setores/{id}
```

| Parâmetro   | Tipo       | Descrição                                   |
| :---------- | :--------- | :------------------------------------------ |
| `id`      | `int` | **Obrigatório**. O ID do setor que deseja consultar |

Response Body: 

```json
  {
    "setorId": 1,
    "tipoSetor": "string",
    "statusSetor": "string",
    "patio": {
      "patioId": 1,
      "localizacaoPatio": "string",
      "nomePatio": "string",
      "descricaoPatio": "string"
    },
    "vagas": [
      {
        "vagaId": 1,
        "numeroVaga": "string",
        "statusOcupada": 1
      }
    ]
  }
```

Códigos de Resposta

| Código HTTP       | Significado                     | Quando ocorre                                                  |
|-------------------|----------------------------------|----------------------------------------------------------------|
| 200 OK            | Requisição bem-sucedida         | Quando o setor é encontrado                     |
| 404 Not Found     | Recurso não encontrado          | Quando o setor especificado não é encontrado               |
| 500 Internal Server Error | Erro interno             | Quando ocorre uma falha inesperada no servidor                 |

### Movimentações

- #### Retorna a lista de movimentações

```http
  GET /movimentacoes
```

Response Body: 

```json
[
  {
    "movimentacaoId": 1,
    "dtEntrada": "2025-05-08T18:51:02.420Z",
    "dtSaida": null,
    "descricaoMovimentacao": "string",
    "moto": {
      "motoId": 1,
      "placaMoto": "string",
      "modeloMoto": "string",
      "situacaoMoto": "string",
      "chassiMoto": "string",
      "cliente": {
        "clienteId": 1,
        "nomeCliente": "string",
        "telefoneCliente": "string",
        "sexoCliente": "string",
        "emailCliente": "string",
        "cpfCliente": "string"
      }
    },
    "vaga": {
      "vagaId": 1,
      "numeroVaga": "string",
      "statusOcupada": 1,
      "setor": {
        "setorId": 1,
        "tipoSetor": "string",
        "statusSetor": "string",
        "patioId": 1
      }
    }
  }
]
```

Códigos de Resposta

| Código HTTP       | Significado                     | Quando ocorre                                                  |
|-------------------|----------------------------------|----------------------------------------------------------------|
| 200 OK            | Requisição bem-sucedida         | Quando as movimentações são encontradas                     |
| 204 No Content    | Sem conteúdo a retornar         | Quando nenhuma movimentação existe  |
| 500 Internal Server Error | Erro interno             | Quando ocorre uma falha inesperada no servidor                 |

- #### Retorna uma movimentação a partir de um ID

```http
  GET /movimentacoes/{id}
```

| Parâmetro   | Tipo       | Descrição                                   |
| :---------- | :--------- | :------------------------------------------ |
| `id`      | `int` | **Obrigatório**. O ID da movimentação que deseja consultar |

Response Body: 

```json
  {
    "movimentacaoId": 1,
    "dtEntrada": "2025-05-08T18:51:02.420Z",
    "dtSaida": null,
    "descricaoMovimentacao": "string",
    "moto": {
      "motoId": 1,
      "placaMoto": "string",
      "modeloMoto": "string",
      "situacaoMoto": "string",
      "chassiMoto": "string",
      "cliente": {
        "clienteId": 1,
        "nomeCliente": "string",
        "telefoneCliente": "string",
        "sexoCliente": "string",
        "emailCliente": "string",
        "cpfCliente": "string"
      }
    },
    "vaga": {
      "vagaId": 1,
      "numeroVaga": "string",
      "statusOcupada": 1,
      "setor": {
        "setorId": 1,
        "tipoSetor": "string",
        "statusSetor": "string",
        "patioId": 1
      }
    }
  }
```

Códigos de Resposta

| Código HTTP       | Significado                     | Quando ocorre                                                  |
|-------------------|----------------------------------|----------------------------------------------------------------|
| 200 OK            | Requisição bem-sucedida         | Quando a movimentação é encontrada                     |
| 404 Not Found     | Recurso não encontrado          | Quando a movimentação especificada não é encontrada              |
| 500 Internal Server Error | Erro interno             | Quando ocorre uma falha inesperada no servidor                 |

- #### Retorna movimentações de uma moto específica

```http
  GET /movimentacoes/por-moto/{id}
```
| Parâmetro   | Tipo       | Descrição                                   |
| :---------- | :--------- | :------------------------------------------ |
| `id`      | `int` | **Obrigatório**. O ID da moto que deseja consultar as movimentações |

Response Body: 

```json
[
  {
    "movimentacaoId": 1,
    "dtEntrada": "2025-05-08T18:51:02.420Z",
    "dtSaida": null,
    "descricaoMovimentacao": "string",
    "moto": {
      "motoId": 1,
      "placaMoto": "string",
      "modeloMoto": "string",
      "situacaoMoto": "string",
      "chassiMoto": "string",
      "cliente": {
        "clienteId": 1,
        "nomeCliente": "string",
        "telefoneCliente": "string",
        "sexoCliente": "string",
        "emailCliente": "string",
        "cpfCliente": "string"
      }
    },
    "vaga": {
      "vagaId": 1,
      "numeroVaga": "string",
      "statusOcupada": 1,
      "setor": {
        "setorId": 1,
        "tipoSetor": "string",
        "statusSetor": "string",
        "patioId": 1
      }
    }
  }
]
```

Códigos de Resposta

| Código HTTP       | Significado                     | Quando ocorre                                                  |
|-------------------|----------------------------------|----------------------------------------------------------------|
| 200 OK            | Requisição bem-sucedida         | Quando as movimentações da moto são encontradas                     |
| 204 No Content    | Sem conteúdo a retornar         | Quando nenhuma movimentação existe  |
| 404 Not Found     | Recurso não encontrado          | Quando a moto especificada não foi encontrada              |
| 500 Internal Server Error | Erro interno             | Quando ocorre uma falha inesperada no servidor                 |

- #### Retorna a ocupação por setor de um pátio com base nas movimentações

```http
  GET /movimentacoes/ocupacao-por-setor/patio/{id}
```
| Parâmetro   | Tipo       | Descrição                                   |
| :---------- | :--------- | :------------------------------------------ |
| `id`      | `int` | **Obrigatório**. O ID do pátio que deseja consultar a ocupação dos setores |

Response Body: 

```json
[
  {
    "setor": "string",
    "totalVagas": 1,
    "motosPresentes": 1
  }
]
```

Códigos de Resposta

| Código HTTP       | Significado                     | Quando ocorre                                                  |
|-------------------|----------------------------------|----------------------------------------------------------------|
| 200 OK            | Requisição bem-sucedida         | Quando a ocupação dos setores é retornada                     |
| 500 Internal Server Error | Erro interno             | Quando ocorre uma falha inesperada no servidor                 |

- #### Cria uma nova movimentação

```http
  POST /movimentacoes
```

Request Body:

```json
{
  "descricaoMovimentacao": "",
  "motoId": 1,
  "vagaId": 1
}
```

Códigos de Resposta

| Código HTTP       | Significado                     | Quando ocorre                                                  |
|-------------------|----------------------------------|----------------------------------------------------------------|
| 201 Created       | Recurso criado com sucesso      | Quando uma movimentação é criada com êxito |
| 400 Bad Request   | Requisição malformada           | Quando os dados enviados estão incorretos ou incompletos       |
| 409 Conflict      | Conflito de estado              | Quando há conflito, como dados duplicados (vaga e moto já presentes em outra movimentação)                     |
| 500 Internal Server Error | Erro interno             | Quando ocorre uma falha inesperada no servidor                 |

- #### Atualiza a data de saída da movimentação

```http
PUT /movimentacoes/{id}/saida
```

| Parâmetro   | Tipo       | Descrição                                   |
| :---------- | :--------- | :------------------------------------------ |
| `id`      | `int` | **Obrigatório**. O ID da movimentação que deseja atualizar a data de saída |

Códigos de Resposta

| Código HTTP       | Significado                     | Quando ocorre                                                  |
|-------------------|----------------------------------|----------------------------------------------------------------|
| 204 No Content    | Sem conteúdo a retornar         | Quando a atualização da data de saída da movimentação é válida, mas não há dados para retornar   |
| 400 Bad Request   | Requisição malformada           | Quando os dados enviados estão incorretos ou incompletos       |
| 404 Not Found     | Recurso não encontrado          | Quando a movimentação especificada não é encontrada                |
| 500 Internal Server Error | Erro interno             | Quando ocorre uma falha inesperada no servidor                 |

## Dockerfile

#### Para que a API funcione na nuvem, é necessário remover o comentário desta linha (este passo foi utilizado na construção da imagem enviada ao Docker Hub (dinozin/sprint1-api-mottion:latest). Se for utilizar a imagem do Docker Hub, ignorar este passo):
```code
//builder.WebHost.UseUrls("http://0.0.0.0:5147");
```

### Estrutura do Dockerfile para a matéria de DEVOPS TOOLS & CLOUD COMPUTING:
#### Deve ser executado na raiz do projeto (não dentro da API).
```
# Etapa 1: build da aplicação usando imagem Alpine
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /app

# Copia os arquivos de projeto e restaura as dependências
COPY Sprint1-API/Sprint1-API.csproj ./Sprint1-API/
RUN dotnet restore ./Sprint1-API/Sprint1-API.csproj

# Copia os demais arquivos e compila a aplicação
COPY Sprint1-API/ ./Sprint1-API/
WORKDIR /app/Sprint1-API
RUN dotnet publish -c Release -o /app/out

# Etapa 2: runtime 
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine
WORKDIR /app

# Cria um usuário sem privilégios
RUN adduser -D -g '' appuser

# Copia a aplicação compilada da etapa anterior
COPY --from=build /app/out . 

# Ajusta permissões para o usuário
RUN chown -R appuser /app

# Define o usuário não-root
USER appuser

# Porta da API
EXPOSE 5147

# Define o ambiente como "Development"
ENV ASPNETCORE_ENVIRONMENT=Development

# Comando para iniciar a aplicação
ENTRYPOINT ["dotnet", "Sprint1-API.dll"]
```

### Observação: A API funciona normalmente na nuvem, mas para que o Scalar funcione na Azure sem precisar criar um novo servidor pelo Client API, é necessário alterar em Program.cs a seguinte linha de código (antes de construir a imagem): 
```
builder.WebHost.UseUrls("http://<ip-publico-da-maquina-virtual>:5147");
```

### E depois construir a imagem e fazer o push em seu Docker Hub:
```
 docker build -t <nome-usuario>/sprint1-api-mottion:latest .
 docker push <nome-usuario>/sprint1-api-mottion:latest
```

## Scripts do Azure CLI

### Criação da VM:

```
UBUNTU="Canonical:ubuntu-24_04-lts:server:24.04.202502210"
VM_SIZE="Standard_B2s"
TASK="1"
LOCATION="brazilsouth"

az group create -g rg-sprint1-api-mottion-2tdsb-$LOCATION -l $LOCATION --tags Sprint=$TASK

az network vnet create -n vnet-sprint1-api-mottion-2tdsb-$LOCATION -g rg-sprint1-api-mottion-2tdsb-$LOCATION --tags Sprint=$TASK

az network vnet subnet create -n snet-sprint1-api-mottion-2tdsb-main -g rg-sprint1-api-mottion-2tdsb-$LOCATION --vnet-name vnet-sprint1-api-mottion-2tdsb-$LOCATION --address-prefixes 10.0.0.0/24

az network nsg create -n nsg-sprint1-api-mottion-2tdsb-$LOCATION -g rg-sprint1-api-mottion-2tdsb-$LOCATION --tags Sprint=$TASK

az network nsg rule create -n ssh --nsg-name nsg-sprint1-api-mottion-2tdsb-$LOCATION --priority 1000 --direction Inbound --destination-address-prefixes VirtualNetwork --destination-port-ranges 22 -g rg-sprint1-api-mottion-2tdsb-$LOCATION --protocol Tcp

az vm create -n vm-sprint1-api-mottion-2tdsb-$LOCATION -g rg-sprint1-api-mottion-2tdsb-$LOCATION --image $UBUNTU --size $VM_SIZE --vnet-name  vnet-sprint1-api-mottion-2tdsb-$LOCATION --subnet snet-sprint1-api-mottion-2tdsb-main --nsg nsg-sprint1-api-mottion-2tdsb-$LOCATION --authentication-type password --admin-username azureuser --admin-password Fiap2TDSB2025
```

### Abertura da Porta 5147 na Azure:
```
az vm open-port --resource-group rg-sprint1-api-mottion-2tdsb-brazilsouth --name vm-sprint1-api-mottion-2tdsb-brazilsouth --port 5147 --priority 1010
```

### Login na VM:
```
ssh azureuser@<ip-publico-vm>
Senha: Fiap2TDSB2025
```

### Instalação do Docker na VM:
```
# Add Docker's official GPG key:
sudo apt-get update
sudo apt-get install ca-certificates curl
sudo install -m 0755 -d /etc/apt/keyrings
sudo curl -fsSL https://download.docker.com/linux/ubuntu/gpg -o /etc/apt/keyrings/docker.asc
sudo chmod a+r /etc/apt/keyrings/docker.asc

# Add the repository to Apt sources:
echo \
  "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.asc] https://download.docker.com/linux/ubuntu \
  $(. /etc/os-release && echo "${UBUNTU_CODENAME:-$VERSION_CODENAME}") stable" | \
  sudo tee /etc/apt/sources.list.d/docker.list > /dev/null
sudo apt-get update

sudo apt-get install docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin

sudo usermod -aG docker $USER
sudo su - azureuser
```

### Criação do Container com base na imagem enviada para o Docker Hub:
```
docker run -d --name sprint1-container -p 5147:5147 dinozin/sprint1-api-mottion:latest
```

### Acessar o Scalar:
```
http://<ip-publico-da-vm>:5147/scalar
```

### Deletar o Resource Group, juntamente com a VM: 
```
az group delete --name rg-sprint1-api-mottion-2tdsb-brazilsouth --yes
```

## Guia para testar Endpoints no Scalar pela Azure criando servidor com o IP público da VM:

![App Screenshot](https://imgur.com/rdplLuU.png)

![App Screenshot](https://imgur.com/Y3KDSYM.png)

![App Screenshot](https://imgur.com/YYXIYKV.png)

![App Screenshot](https://imgur.com/QgJAJGn.png)

![App Screenshot](https://imgur.com/SdVsc81.png)
