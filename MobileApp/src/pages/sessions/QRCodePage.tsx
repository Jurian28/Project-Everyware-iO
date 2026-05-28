import { useEffect, useState } from "react";
import QRCode from "qrcode";
import { Pressable, Text, View, Image } from "react-native"
import { useNavigation, useRoute } from '@react-navigation/native';
import AppLayout from "../../layouts/AppLayout"
import sessionStyles from "../../styles/sessionStyles";
import config from "../../config";
import Colors from "../../enums/colors";
import { getTextColorForBackground } from "../../utils/colors";
import { Session } from "../../services/SessionService";
import { formatTime } from "../../utils/dates";
import AuthService from "../../services/AuthService";

function Header({ session }: { session: Session }) {
  const navigation = useNavigation<any>();

  return (
    <View style={sessionStyles.qrPageHeader}>
      <Pressable 
        onPress={() => navigation.navigate('SessionView', { sessionId: session.sessionId })} 
        style={sessionStyles.backButton}
      >
        <Text style={sessionStyles.backButton}>&larr;</Text>
      </Pressable>
    </View>
  )
}

function QRCodeCard({ eventMainColorHex, session, qrCode }: { eventMainColorHex: string, session: Session, qrCode: string }) {
  const textColor = getTextColorForBackground(eventMainColorHex || Colors.DEFAULT_BUTTON_COLOR);
  
  return (
    <View style={[sessionStyles.qrCodeCard, { backgroundColor: eventMainColorHex || Colors.DEFAULT_BUTTON_COLOR }]}>
      <View style={sessionStyles.qrCodeBox}>
        {qrCode ? (
          <Image 
            source={{ uri: qrCode }} 
            style={sessionStyles.qrCodeImage} 
          />
        ) : (
          <Text style={sessionStyles.qrCodeLoadingText}>Generating QR Code...</Text>
        )}
      </View>
      
      <View style={sessionStyles.qrCodeInfoSection}>
        <Text style={[sessionStyles.qrCodeEventTitle, { color: textColor }]}>
          {session.title}
        </Text>
        <Text style={[sessionStyles.qrCodeEventDate, { color: textColor }]}>
          {formatTime(session.startTime)}
        </Text>
      </View>
    </View>
  )
}

export default function QRCodePage() {
  const route = useRoute<any>();
  const { eventMainColorHex, session }: { eventMainColorHex: string; session: Session } = route.params;
  const [qrCode, setQrCode] = useState<string>('');
  
  useEffect(() => {
    async function GenerateQRCode() {
      try {
        const userId = await AuthService.getUserId();
        const qrInput = `io-event-connecter://session/${session.sessionId}/user/${userId}`;
        const qr = await QRCode.toDataURL(
          qrInput, 
          {  
            width: 300, 
            margin: 2 
          }
        );
        setQrCode(qr);
      } catch (error) {
        console.error('Error generating QR code:', error);
      }
    }

    GenerateQRCode();
  }, [session.sessionId]);
    
  return (
    <AppLayout>
      <View style={sessionStyles.qrPadding}>
        <Header session={session} /> 

        <View style={sessionStyles.qrCodePageWrapper}>
          <Text style={sessionStyles.qrCodeTitle}>Attendance QR Code</Text>
          
          <QRCodeCard eventMainColorHex={eventMainColorHex} session={session} qrCode={qrCode} />

          <Text style={sessionStyles.qrCodeInstructions}>
            Scan this QR code to mark your attendance
          </Text>
        </View>
      </View>
    </AppLayout>
  )
}