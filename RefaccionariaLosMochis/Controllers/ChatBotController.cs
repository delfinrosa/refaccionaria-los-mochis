using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace RefaccionariaLosMochis.Controllers
{
    public class ChatBotController : Controller
    {

        //public async Task<IActionResult> EnviarPregunta(string pregunta)
        //{   
        //    try
        //    {
        //        var apiKey = "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";
        //        var url = "https://api.openai.com/v1/chat/completions"; // Asegúrate de que la URL sea correcta

        //        string manual = "La aplicación tiene cuatro tipos de permisos:\r\n\r\nAdministrador: Acceso total.\r\nInventario: Permisos para gestionar marcas, líneas, productos, clientes, proveedores, almacén y compras.\r\nVendedor: Solo puede acceder a la vista de ventas para realizar transacciones.\r\nCajero: Acceso limitado a la caja para cobrar ventas y procesar devoluciones.\r\nConsejos para la gestión de productos:\r\n\r\nSi no puedes eliminar una marca o producto, puede ser debido a que está relacionado con otros elementos. En lugar de eliminar, considera deshabilitarlo o ocultarlo.\r\nPara dar de alta un producto, verifica:\r\nLa línea a la que estará relacionada.\r\nQue la marca esté registrada.\r\nQue la ubicación (rack y sección) en el almacén esté registrada.\r\nRegistro de Otros Elementos:\r\n\r\nPara registrar marcas, líneas, clientes, productos, proveedores, etc., necesitas el permiso de administrador o inventario.\r\nEn la opción de inventario, selecciona qué quieres registrar (marca, línea, cliente, etc.). Haz clic en el botón azul \"Nuevo\" y completa los campos requeridos, que aparecerán en verde. Si los campos están en azul, estás en modo editor, editando un registro existente. Cada vez que accedas a estas páginas, se carga el último registro registrado.\r\nBúsqueda de Elementos:\r\n\r\nPara buscar algo, habilita un switch en la parte superior de la tabla. Este switch desplegará opciones o filtros. Activa el filtro, ingresa lo que deseas buscar y haz clic en el botón \"Aplicar\". La tabla se llenará con los elementos filtrados.";

        //        var requestIA = new RequestOpenIA()
        //        {
        //            model = "gpt-3.5-turbo", // Cambia el modelo si es necesario
        //            messages = new List<Message>()
        //            {
        //                new Message()
        //                {
        //                    role = "user",
        //                    content = manual // Usa la pregunta proporcionada
        //                },
        //                new Message()
        //                {
        //                    role = "user",
        //                    content = pregunta // Usa la pregunta proporcionada
        //                }
        //            }
        //        };

        //        var json = JsonConvert.SerializeObject(requestIA); // Asegúrate de que Newtonsoft.Json esté instalado

        //        var body = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        //        var client = new HttpClient();
        //        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey); // Corrige "Autherization" a "Authorization"
        //        var response = await client.PostAsync(url, body);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var bodyRequest = await response.Content.ReadAsStringAsync();
        //            dynamic jsonResponse = JsonConvert.DeserializeObject(bodyRequest); // Deserializa el JSON a un objeto dinámico
        //            var completionText = jsonResponse.choices[0].message.content; // Accede directamente al contenido
        //            return Ok(new { respuesta = completionText }); // Devuelve el contenido de la respuesta
        //        }
        //        else
        //        {
        //            return StatusCode((int)response.StatusCode, "Error al obtener respuesta de ChatGPT.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Error inesperado: {ex.Message}");
        //    }
        //}
        public async Task<JsonResult> EnviarPregunta(string pregunta)
        {   
            try
            {
                var apiKey = "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";
                var url = "https://api.openai.com/v1/chat/completions"; // Asegúrate de que la URL sea correcta

                string manual = "La aplicación tiene cuatro tipos de permisos:\r\n\r\nAdministrador: Acceso total.\r\nInventario: Permisos para gestionar marcas, líneas, productos, clientes, proveedores, almacén y compras.\r\nVendedor: Solo puede acceder a la vista de ventas para realizar transacciones.\r\nCajero: Acceso limitado a la caja para cobrar ventas y procesar devoluciones.\r\nConsejos para la gestión de productos:\r\n\r\nSi no puedes eliminar una marca o producto, puede ser debido a que está relacionado con otros elementos. En lugar de eliminar, considera deshabilitarlo o ocultarlo.\r\nPara dar de alta un producto, verifica:\r\nLa línea a la que estará relacionada.\r\nQue la marca esté registrada.\r\nQue la ubicación (rack y sección) en el almacén esté registrada.\r\nRegistro de Otros Elementos:\r\n\r\nPara registrar marcas, líneas, clientes, productos, proveedores, etc., necesitas el permiso de administrador o inventario.\r\nEn la opción de inventario, selecciona qué quieres registrar (marca, línea, cliente, etc.). Haz clic en el botón azul \"Nuevo\" y completa los campos requeridos, que aparecerán en verde. Si los campos están en azul, estás en modo editor, editando un registro existente. Cada vez que accedas a estas páginas, se carga el último registro registrado.\r\nBúsqueda de Elementos:\r\n\r\nPara buscar algo, habilita un switch en la parte superior de la tabla. Este switch desplegará opciones o filtros. Activa el filtro, ingresa lo que deseas buscar y haz clic en el botón \"Aplicar\". La tabla se llenará con los elementos filtrados.";

                var requestIA = new RequestOpenIA()
                {
                    model = "gpt-3.5-turbo", // Cambia el modelo si es necesario
                    messages = new List<Message>()
                    {
                        new Message()
                        {
                            role = "user",
                            content = manual // Usa la pregunta proporcionada
                        },
                        new Message()
                        {
                            role = "user",
                            content = pregunta // Usa la pregunta proporcionada
                        }
                    }
                };

                var json = JsonConvert.SerializeObject(requestIA); // Asegúrate de que Newtonsoft.Json esté instalado

                var body = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey); // Corrige "Autherization" a "Authorization"
                var response = await client.PostAsync(url, body);

                if (response.IsSuccessStatusCode)
                {
                    var bodyRequest = await response.Content.ReadAsStringAsync();
                    dynamic jsonResponse = JsonConvert.DeserializeObject(bodyRequest); // Deserializa el JSON a un objeto dinámico
                    var completionText = jsonResponse.choices[0].message.content; // Accede directamente al contenido
                    
                    return Json(new { respuesta = ""+completionText+" " }); ;

                }
                else
                {
                    return Json(new { respuesta = "Error al obtener respuesta de ChatGPT." }); ;

                }
            }
            catch (Exception ex)
            {
                return Json(new { respuesta = $"Error inesperado: {ex.Message}" }); ;

            }
        }

        public class RequestOpenIA
        {
            public string model { get; set; }
            public List<Message> messages { get; set; }
        }

        public class Message
        {
            public string role { get; set; }
            public string content { get; set; }
        }
    }
}
