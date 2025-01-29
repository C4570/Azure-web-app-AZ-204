import sys
import chardet
import PyPDF2
import os

def detectar_codificacion(nombre_archivo):
    with open(nombre_archivo, 'rb') as f:
        resultado = chardet.detect(f.read(100000))  
    return resultado['encoding']

def procesar_txt(nombre_archivo):
    """
    Detecta la codificación con chardet y lee un archivo de texto.
    """
    codificacion = detectar_codificacion(nombre_archivo)
    with open(nombre_archivo, 'r', encoding=codificacion) as f:
        contenido = f.read()
    return (
        f"Archivo {nombre_archivo} procesado con éxito "
        f"(Codificación: {codificacion}).\nContenido:\n{contenido}"
    )

def procesar_pdf(nombre_archivo):
    """
    Lee un archivo PDF y extrae todo su texto.
    """
    with open(nombre_archivo, 'rb') as archivo_pdf:
        reader = PyPDF2.PdfReader(archivo_pdf)
        texto_total = []
        for num_pagina, pagina in enumerate(reader.pages):
            texto = pagina.extract_text()
            if texto:
                texto_total.append(f"--- Página {num_pagina + 1} ---\n{texto}")
            else:
                texto_total.append(f"--- Página {num_pagina + 1} (sin texto reconocible) ---")
    return "\n".join(texto_total)

def procesar_archivo(nombre_archivo):
    """
    Decide si el archivo es PDF o TXT (u otro) y llama a la función adecuada.
    """
    extension = os.path.splitext(nombre_archivo)[1].lower()
    try:
        if extension == '.pdf':
            contenido = procesar_pdf(nombre_archivo)
            return f"Archivo PDF '{nombre_archivo}' procesado.\n{contenido}"
        elif extension == '.txt':
            contenido = procesar_txt(nombre_archivo)
            return contenido
        else:
            return f"No se cómo procesar la extensión '{extension}'."
    except Exception as e:
        return f"Error procesando archivo: {str(e)}"

if __name__ == "__main__":
    if len(sys.argv) > 1:
        archivo = sys.argv[1]
        print(procesar_archivo(archivo))
    else:
        print("Error: No se proporcionó un archivo")