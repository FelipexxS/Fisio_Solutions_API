using Microsoft.EntityFrameworkCore;
using Fisio_Solutions_API.Models;

namespace Fisio_Solutions_API.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            // Cria as tabelas caso o arquivo SQLite fisiosolutions.db já existisse previamente de uma versão anterior
            context.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS ""Exercicios"" (
                    ""Id"" INTEGER NOT NULL CONSTRAINT ""PK_Exercicios"" PRIMARY KEY AUTOINCREMENT,
                    ""Titulo"" TEXT NOT NULL,
                    ""RegiaoCorpo"" TEXT NOT NULL,
                    ""Objetivo"" TEXT NULL,
                    ""DescricaoPassoAPasso"" TEXT NULL,
                    ""ImagemIlustracaoUrl"" TEXT NULL,
                    ""RepeticaoSugerida"" TEXT NULL,
                    ""DuracaoSegundos"" INTEGER NOT NULL DEFAULT 30,
                    ""Ativo"" INTEGER NOT NULL DEFAULT 1
                );

                CREATE TABLE IF NOT EXISTS ""Listas_Exercicios"" (
                    ""Id"" INTEGER NOT NULL CONSTRAINT ""PK_Listas_Exercicios"" PRIMARY KEY AUTOINCREMENT,
                    ""UsuarioId"" INTEGER NOT NULL,
                    ""NomeLista"" TEXT NOT NULL,
                    ""Categoria"" TEXT NULL,
                    ""CreatedAt"" TEXT NOT NULL,
                    CONSTRAINT ""FK_Listas_Exercicios_Users_UsuarioId"" FOREIGN KEY (""UsuarioId"") REFERENCES ""Users"" (""Id"") ON DELETE CASCADE
                );

                CREATE TABLE IF NOT EXISTS ""Lista_Exercicio_Itens"" (
                    ""Id"" INTEGER NOT NULL CONSTRAINT ""PK_Lista_Exercicio_Itens"" PRIMARY KEY AUTOINCREMENT,
                    ""ListaId"" INTEGER NOT NULL,
                    ""ExercicioId"" INTEGER NOT NULL,
                    ""OrdemPosicao"" INTEGER NOT NULL,
                    CONSTRAINT ""FK_Lista_Exercicio_Itens_Exercicios_ExercicioId"" FOREIGN KEY (""ExercicioId"") REFERENCES ""Exercicios"" (""Id"") ON DELETE RESTRICT,
                    CONSTRAINT ""FK_Lista_Exercicio_Itens_Listas_Exercicios_ListaId"" FOREIGN KEY (""ListaId"") REFERENCES ""Listas_Exercicios"" (""Id"") ON DELETE CASCADE
                );

                CREATE TABLE IF NOT EXISTS ""Dicas_Ergonomia"" (
                    ""Id"" INTEGER NOT NULL CONSTRAINT ""PK_Dicas_Ergonomia"" PRIMARY KEY AUTOINCREMENT,
                    ""Titulo"" TEXT NOT NULL,
                    ""Conteudo"" TEXT NOT NULL,
                    ""ImagemIlustracaoUrl"" TEXT NULL,
                    ""Ativo"" INTEGER NOT NULL DEFAULT 1,
                    ""CreatedAt"" TEXT NOT NULL
                );
            ");

            if (!context.Exercicios.Any())
            {

            var exercicios = new List<Exercicio>
            {
                // Coluna
                new Exercicio
                {
                    Titulo = "Cat-Cow (Gato e Vaca)",
                    RegiaoCorpo = "Coluna",
                    Objetivo = "alongamento",
                    DescricaoPassoAPasso = "Apoie os joelhos e mãos no chão em quatro apoios. Ao inspirar, afunde a coluna suavemente em direção ao chão e eleve a cabeça (posição da vaca). Ao expirar, curve a coluna para cima em direção ao teto, recolhendo o pescoço (posição do gato). Repita suavemente de 10 a 12 vezes.",
                    ImagemIlustracaoUrl = "exercise_ilustration_1",
                    RepeticaoSugerida = "10 a 12 repetições",
                    DuracaoSegundos = 30,
                    Ativo = true
                },
                new Exercicio
                {
                    Titulo = "Ponte Pélvica (Glute Bridge)",
                    RegiaoCorpo = "Coluna",
                    Objetivo = "força",
                    DescricaoPassoAPasso = "Deite-se de costas com os joelhos dobrados e pés firmes no chão. Contraia o abdômen e eleve o quadril até formar uma linha reta dos ombros aos joelhos. Mantenha a posição por 3 a 5 segundos e retorne devagar. Faça 3 séries de 10 repetições.",
                    ImagemIlustracaoUrl = "exercise_ilustration_2",
                    RepeticaoSugerida = "3 séries de 10 repetições",
                    DuracaoSegundos = 45,
                    Ativo = true
                },
                new Exercicio
                {
                    Titulo = "Abraço de Joelhos no Peito",
                    RegiaoCorpo = "Coluna",
                    Objetivo = "alongamento",
                    DescricaoPassoAPasso = "Deite-se de costas em uma superfície confortável. Dobre os dois joelhos e traga-os em direção ao peito, abraçando as pernas com as mãos. Mantenha a lombar bem apoiada e faça respirações profundas por 30 segundos.",
                    ImagemIlustracaoUrl = "exercise_ilustration_3",
                    RepeticaoSugerida = "1 série de 30 segundos",
                    DuracaoSegundos = 30,
                    Ativo = true
                },
                new Exercicio
                {
                    Titulo = "Exercício do Super-Homem",
                    RegiaoCorpo = "Coluna",
                    Objetivo = "força",
                    DescricaoPassoAPasso = "Deite-se de barriga para baixo com os braços estendidos à frente. Eleve o peito, os braços e as pernas a poucos centímetros do chão, mantendo o pescoço neutro. Mantenha por 3 segundos e retorne. Faça 3 séries de 8 repetições.",
                    ImagemIlustracaoUrl = "exercise_ilustration_4",
                    RepeticaoSugerida = "3 séries de 8 repetições",
                    DuracaoSegundos = 40,
                    Ativo = true
                },
                new Exercicio
                {
                    Titulo = "Rotação de Tronco Deitado",
                    RegiaoCorpo = "Coluna",
                    Objetivo = "alongamento",
                    DescricaoPassoAPasso = "Deite-se de costas com os braços abertos em T. Dobre os joelhos e deixe-os cair suavemente para o lado direito, enquanto olha para o lado esquerdo. Mantenha por 20 a 30 segundos e inverta o lado.",
                    ImagemIlustracaoUrl = "exercise_ilustration_5",
                    RepeticaoSugerida = "20 a 30 segundos de cada lado",
                    DuracaoSegundos = 60,
                    Ativo = true
                },

                // Pescoço
                new Exercicio
                {
                    Titulo = "Inclinação Lateral do Pescoço",
                    RegiaoCorpo = "Pescoço",
                    Objetivo = "alongamento",
                    DescricaoPassoAPasso = "Sentado ereto, incline a cabeça trazendo a orelha direita em direção ao ombro direito. Use a mão direita para aplicar uma leve e suave pressão sobre a cabeça. Mantenha por 25 segundos e repita no lado esquerdo.",
                    ImagemIlustracaoUrl = "exercise_ilustration_1",
                    RepeticaoSugerida = "25 segundos cada lado",
                    DuracaoSegundos = 50,
                    Ativo = true
                },
                new Exercicio
                {
                    Titulo = "Retração Cervical (Queixo Duplo)",
                    RegiaoCorpo = "Pescoço",
                    Objetivo = "força",
                    DescricaoPassoAPasso = "Sentado com as costas retas, deslize a cabeça horizontalmente para trás (como se quisesse fazer um queixo duplo), mantendo o olhar para a frente. Segure por 3 segundos e relaxe. Realize 10 repetições.",
                    ImagemIlustracaoUrl = "exercise_ilustration_2",
                    RepeticaoSugerida = "10 repetições",
                    DuracaoSegundos = 30,
                    Ativo = true
                },
                new Exercicio
                {
                    Titulo = "Rotação Cervical Suave",
                    RegiaoCorpo = "Pescoço",
                    Objetivo = "alongamento",
                    DescricaoPassoAPasso = "Mantenha os ombros relaxados e gire a cabeça devagar olhando por cima do ombro direito até sentir um alongamento leve. Mantenha por 15 segundos e gire para o esquerdo. Repita 3 vezes de cada lado.",
                    ImagemIlustracaoUrl = "exercise_ilustration_3",
                    RepeticaoSugerida = "3 vezes de cada lado",
                    DuracaoSegundos = 30,
                    Ativo = true
                },
                new Exercicio
                {
                    Titulo = "Alongamento Flexor Anterior",
                    RegiaoCorpo = "Pescoço",
                    Objetivo = "alongamento",
                    DescricaoPassoAPasso = "Sentado com a postura ereta, incline suavemente a cabeça para trás, elevando o queixo em direção ao teto. Abra e feche a boca devagar para intensificar o alongamento na parte frontal do pescoço. Mantenha por 20 segundos.",
                    ImagemIlustracaoUrl = "exercise_ilustration_4",
                    RepeticaoSugerida = "20 segundos",
                    DuracaoSegundos = 20,
                    Ativo = true
                },

                // Braços
                new Exercicio
                {
                    Titulo = "Alongamento de Flexores do Punho",
                    RegiaoCorpo = "Braços",
                    Objetivo = "alongamento",
                    DescricaoPassoAPasso = "Estenda o braço direito à frente com a palma voltada para cima. Com a mão esquerda, puxe os dedos para baixo e para trás em direção ao corpo, mantendo o cotovelo esticado. Mantenha por 25 segundos de cada lado.",
                    ImagemIlustracaoUrl = "exercise_ilustration_4",
                    RepeticaoSugerida = "25 segundos cada lado",
                    DuracaoSegundos = 50,
                    Ativo = true
                },
                new Exercicio
                {
                    Titulo = "Alongamento de Tríceps sobre a Cabeça",
                    RegiaoCorpo = "Braços",
                    Objetivo = "alongamento",
                    DescricaoPassoAPasso = "Eleve o braço direito, dobre o cotovelo e leve a mão atrás das costas. Com a mão esquerda, segure o cotovelo direito e puxe-o suavemente para trás. Mantenha a posição por 20 segundos em cada braço.",
                    ImagemIlustracaoUrl = "exercise_ilustration_3",
                    RepeticaoSugerida = "20 segundos cada lado",
                    DuracaoSegundos = 40,
                    Ativo = true
                },
                new Exercicio
                {
                    Titulo = "Alongamento Cruzado de Ombro",
                    RegiaoCorpo = "Braços",
                    Objetivo = "alongamento",
                    DescricaoPassoAPasso = "Traga o braço direito estendido horizontalmente sobre o peito. Com o braço esquerdo, pressione-o suavemente contra o corpo até sentir o alongamento no ombro e tríceps. Mantenha por 20 segundos de cada lado.",
                    ImagemIlustracaoUrl = "exercise_ilustration_1",
                    RepeticaoSugerida = "20 segundos cada lado",
                    DuracaoSegundos = 40,
                    Ativo = true
                },
                new Exercicio
                {
                    Titulo = "Rotação de Punhos e Antebraço",
                    RegiaoCorpo = "Braços",
                    Objetivo = "alongamento",
                    DescricaoPassoAPasso = "Feche as mãos levemente em punho e faça movimentos circulares lentos 10 vezes no sentido horário e 10 vezes no sentido anti-horário. Abra e feche a palma das mãos energicamente 15 vezes.",
                    ImagemIlustracaoUrl = "exercise_ilustration_2",
                    RepeticaoSugerida = "10 rotações cada sentido",
                    DuracaoSegundos = 30,
                    Ativo = true
                },

                // Pernas
                new Exercicio
                {
                    Titulo = "Alongamento de Isquiotibiais",
                    RegiaoCorpo = "Pernas",
                    Objetivo = "alongamento",
                    DescricaoPassoAPasso = "Sentado na ponta de uma cadeira, estenda uma perna à frente apoiando o calcanhar no chão. Mantenha a coluna ereta e incline o tronco suavemente para a frente a partir do quadril. Mantenha por 30 segundos em cada perna.",
                    ImagemIlustracaoUrl = "exercise_ilustration_5",
                    RepeticaoSugerida = "30 segundos cada perna",
                    DuracaoSegundos = 60,
                    Ativo = true
                },
                new Exercicio
                {
                    Titulo = "Alongamento de Panturrilha na Parede",
                    RegiaoCorpo = "Pernas",
                    Objetivo = "alongamento",
                    DescricaoPassoAPasso = "Apoie as mãos em uma parede e dê um passo para trás com a perna direita, mantendo o calcanhar no chão e o joelho esticado. Dobre o joelho esquerdo para a frente até sentir a panturrilha direita. Mantenha 30 segundos de cada lado.",
                    ImagemIlustracaoUrl = "exercise_ilustration_1",
                    RepeticaoSugerida = "30 segundos cada lado",
                    DuracaoSegundos = 60,
                    Ativo = true
                },
                new Exercicio
                {
                    Titulo = "Alongamento de Quadríceps em Pé",
                    RegiaoCorpo = "Pernas",
                    Objetivo = "alongamento",
                    DescricaoPassoAPasso = "Em pé, apoie uma mão na parede para equilíbrio. Dobre o joelho direito trazendo o calcanhar ao glúteo e segure o tornozelo com a mão direita. Mantenha os joelhos alinhados e o corpo ereto por 25 segundos. Inverta o lado.",
                    ImagemIlustracaoUrl = "exercise_ilustration_2",
                    RepeticaoSugerida = "25 segundos cada perna",
                    DuracaoSegundos = 50,
                    Ativo = true
                },
                new Exercicio
                {
                    Titulo = "Elevação de Calcanhares",
                    RegiaoCorpo = "Pernas",
                    Objetivo = "força",
                    DescricaoPassoAPasso = "Em pé com os pés afastados na largura do quadril, eleve-se sobre a ponta dos pés contraindo a panturrilha no topo. Desça suavemente até encostar o calcanhar no chão. Realize 3 séries de 12 repetições.",
                    ImagemIlustracaoUrl = "exercise_ilustration_3",
                    RepeticaoSugerida = "3 séries de 12 repetições",
                    DuracaoSegundos = 45,
                    Ativo = true
                }
            };

            context.Exercicios.AddRange(exercicios);
            context.SaveChanges();
        }

        if (!context.DicasErgonomia.Any())
        {
            var dicas = new List<DicaErgonomia>
            {
                new DicaErgonomia
                {
                    Titulo = "Altura Correta do Monitor e Distância dos Olhos",
                    ImagemIlustracaoUrl = "ergonomics_banner",
                    Conteudo = "Posicionar o monitor adequadamente previne dores no pescoço e fadiga visual durante longas jornadas de trabalho.\n\nPrincipais orientações:\n1. Mantenha o topo da tela na altura dos seus olhos ou ligeiramente abaixo.\n2. A distância entre a tela e seus olhos deve ser equivalente ao comprimento do seu braço (aproximadamente 50 a 70 cm).\n3. Incline levemente a tela para trás (10 a 20 graus) para evitar reflexos da iluminação ambiente.",
                    Ativo = true,
                    CreatedAt = DateTime.UtcNow
                },
                new DicaErgonomia
                {
                    Titulo = "Ajuste da Cadeira e Postura Sentada",
                    ImagemIlustracaoUrl = "exercise_ilustration_1",
                    Conteudo = "Uma cadeira bem regulada distribui o peso do corpo de forma uniforme e reduz a pressão sobre a coluna vertebral.\n\nComo ajustar sua cadeira:\n1. Ajuste a altura do assento para que os joelhos fiquem dobrados em um ângulo de 90 graus e ambos os pés fiquem totalmente apoiados no chão.\n2. Utilize o suporte lombar da cadeira para manter a curva natural da parte inferior das costas.\n3. Ajuste os apoios de braço para que os cotovelos fiquem em 90 graus e os ombros permaneçam relaxados.",
                    Ativo = true,
                    CreatedAt = DateTime.UtcNow
                },
                new DicaErgonomia
                {
                    Titulo = "Posicionamento do Teclado e Mouse (Prevenção LER/DORT)",
                    ImagemIlustracaoUrl = "exercise_ilustration_2",
                    Conteudo = "LER (Lesão por Esforço Repetitivo) e DORT (Distúrbios Osteomusculares Relacionados ao Trabalho) são frequentes em quem digita por muitas horas.\n\nRecomendações fundamentais:\n1. Mantenha o teclado e o mouse no mesmo nível e próximos ao corpo.\n2. Evite dobrar os punhos para cima ou para os lados durante a digitação; os punhos devem permanecer neutros e retos.\n3. Utilize descansos de punho macios e mouses ergonômicos ou verticais se sentir desconforto frequente.",
                    Ativo = true,
                    CreatedAt = DateTime.UtcNow
                },
                new DicaErgonomia
                {
                    Titulo = "A Regra 20-20-20 para Alívio do Cansaço Visual",
                    ImagemIlustracaoUrl = "exercise_ilustration_3",
                    Conteudo = "Olhar para telas por longos períodos reduz a frequência de piscadas, gerando ressecamento nos olhos e dores de cabeça.\n\nComo praticar a regra 20-20-20:\n1. A cada 20 minutos de trabalho em frente à tela...\n2. Olhe para um objeto ou ponto situado a pelo menos 6 metros (20 pés) de distância.\n3. Mantenha o foco nesse ponto distante por 20 segundos.\nIsso permite que os músculos oculares relaxem completamente.",
                    Ativo = true,
                    CreatedAt = DateTime.UtcNow
                },
                new DicaErgonomia
                {
                    Titulo = "Pausas Ativas e Microintervalos Durante a Jornada",
                    ImagemIlustracaoUrl = "exercise_ilustration_4",
                    Conteudo = "Permanecer na mesma posição por mais de 1 hora consecutiva reduz a circulação sanguínea e aumenta a rigidez muscular.\n\nHábitos saudáveis diários:\n1. Faça micropausas de 2 a 3 minutos a cada hora para se levantar, caminhar um pouco e beber água.\n2. Realize alongamentos leves de pescoço, ombros e punhos durante as pausas.\n3. Alterne entre trabalhar sentado e em pé caso possua uma mesa com regulagem de altura.",
                    Ativo = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            context.DicasErgonomia.AddRange(dicas);
            context.SaveChanges();
        }
    }
}
}


