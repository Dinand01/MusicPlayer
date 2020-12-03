using AutoMapper;
using Grpc.Core;
using MusicPlayer.Core.Models;
using MusicPlayer.Core.Player;
using System;
using System.Threading.Tasks;

namespace MusicPlayer.Communication.Grpc.Service
{
    internal class MusicPlayerService : MusicPlayerServer.MusicPlayerServerBase, IMusicPlayerServer
    {
        private readonly IMusicPlayer _musicPlayer;
        private readonly IMapper _mapper;

        public event Action<string> ClientDisconnected;
        public event Action<Client> ClientConnected;

        public MusicPlayerService(IMusicPlayer musicPlayer, IMapper mapper)
        {
            _musicPlayer = musicPlayer;
            _mapper = mapper;
        }

        public override Task GetServerStream(Empty request, IServerStreamWriter<StreamingMessage> responseStream, ServerCallContext context)
        {
            var client = new Client
            {
                Peer = context.Peer
            };

            client.SendMessageEvent += async (Core.Communication.StreamingMessages.StreamingMessage message) =>
            {
                var protoMessage = new StreamingMessage();
                switch (message)
                {
                    case Core.Communication.StreamingMessages.GoodByeMessage goodbyeMessage:
                        protoMessage.GoodBye = _mapper.Map<GoodByeMessage>(goodbyeMessage);
                        break;
                    case Core.Communication.StreamingMessages.PlayVideoMessage playVideoMessage:
                        protoMessage.PlayVideoMessage = _mapper.Map<PlayVideoMessage>(playVideoMessage);
                        break;
                    case Core.Communication.StreamingMessages.SeekSongMessage seekSongMessage:
                        protoMessage.SeekSongMessage = _mapper.Map<SeekSongMessage>(seekSongMessage);
                        break;
                    case Core.Communication.StreamingMessages.SeekVideoMessage seekVideoMessage:
                        protoMessage.SeekVideoMessage = _mapper.Map<SeekVideoMessage>(seekVideoMessage);
                        break;
                    case Core.Communication.StreamingMessages.SetRadioInformationMessage setRadioInformationMessage:
                        protoMessage.SetRadioInformationMessage = _mapper.Map<SetRadioInformationMessage>(setRadioInformationMessage);
                        break;
                    case Core.Communication.StreamingMessages.SetSongInformationMessage setSongInfoMessage:
                        protoMessage.SetSongInformationMessage = _mapper.Map<SetSongInformationMessage>(setSongInfoMessage);
                        break;
                    case Core.Communication.StreamingMessages.StreamDataMessage streamMessage:
                        protoMessage.StreamDataMessage = _mapper.Map<StreamDataMessage>(streamMessage);
                        break;
                    case Core.Communication.StreamingMessages.TogglePlayMessage togglePlayMessage:
                        protoMessage.TogglePlayMessage = _mapper.Map<TogglePlayMessage>(togglePlayMessage);
                        break;
                }

                await responseStream.WriteAsync(protoMessage);
            };

            ClientConnected?.Invoke(client);

            return Task.CompletedTask;
        }

        public override Task<PositionInfo> GetCurrentPosition(Empty request, ServerCallContext context)
        {
            int? position = _musicPlayer?.GetSongPosition();
            return Task.FromResult(new PositionInfo
            {
                HasPosition = position.HasValue,
                Position = position.GetValueOrDefault()
            });
        }

        public override Task<Empty> GoodBye(Empty request, ServerCallContext context)
        {
            ClientDisconnected?.Invoke(context.Peer);
            return base.GoodBye(request, context);
        }
    }
}
