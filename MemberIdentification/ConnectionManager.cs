using System;
using System.Diagnostics;
using System.Linq;

using RfidBus.Primitives.Messages.Readers;
using RfidBus.Primitives.Network;
using RfidBus.Serializers.Ws;

using RfidCenter.Basic;
using RfidCenter.Basic.Arguments;

namespace MemberIdentification
{
    internal sealed class ConnectionManager
    {
        private RfidBusClient _client;
        private Guid _listenReaderId = Guid.Empty;

        static ConnectionManager()
        {
            Instance = new ConnectionManager();
        }

        private ConnectionManager()
        {
        }

        public static ConnectionManager Instance
        {
            [DebuggerStepThrough] get;
        }

        public void Initialize()
        {
            try
            {
                this._listenReaderId = Guid.Parse(Properties.Settings.Default.ListenReader);

                var pbCommunication = new WsCommunicationDescription();
                var config = new ParametersValues(pbCommunication.GetClientConfiguration());
                config.SetValue(ConfigConstants.PARAMETER_HOST, Properties.Settings.Default.BusHost);
                config.SetValue(ConfigConstants.PARAMETER_PORT, Properties.Settings.Default.BusPort);

                this._client = new RfidBusClient(pbCommunication, config);

                if (!this._client.Authorize(Properties.Settings.Default.BusLogin,
                                            Properties.Settings.Default.BusPassword))
                    throw new BaseException(RfidErrorCode.InvalidLoginAndPassword);

                this._client.ReceivedEvent += this.ClientOnReceivedEvent;

                this._client.SendRequest(new SubscribeToReader(this._listenReaderId));
                this._client.SendRequest(new StartReading(this._listenReaderId));
            }
            catch (BaseException ex)
            {
                BaseTools.Log.Error("Can't connect to RFID Bus at {0}:{1} as {2}:{3}. Error: {4}.",
                                    Properties.Settings.Default.BusHost,
                                    Properties.Settings.Default.BusPort,
                                    Properties.Settings.Default.BusLogin,
                                    Properties.Settings.Default.BusPassword,
                                    (RfidErrorCode) ex.ErrorCode);
                throw;
            }
        }

        private void ClientOnReceivedEvent(object sender, ReceivedEventEventArgs args)
        {
            if (args.EventMessage is TransponderFoundEvent)
            {
                var msg = (TransponderFoundEvent) args.EventMessage;

                if (this._listenReaderId != msg.ReaderRecord.Id)
                    return;

                this.OnTagsFound(new TagsEventArgs(msg.Transponders.Select(rec => rec.IdAsString).ToArray()));
            }
        }

        public event EventHandler<TagsEventArgs> TagsFound;

        private void OnTagsFound(TagsEventArgs e)
        {
            this.TagsFound?.Invoke(this, e);
        }

        public void Close()
        {
            this._client?.Close();
        }
    }
}