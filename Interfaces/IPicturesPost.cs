using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UGB.Proyecto.Final.Interfaces
{
    public interface IPicturesPost
    {
        public Task<int> ContarImagenesPorPostUsuario(int idUsuario, int idPost);
        //ctx.imagenes_post.Where(x=>x.id_usuario == idUsuario && x.id_post = idPost).Count();
    }
}