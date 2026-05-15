import { useEffect, useState } from "react";
import QRCode from "qrcode";
import { Pressable, Text, View, Image } from "react-native"
import { Event } from '../../services/EventService';
import { useNavigation, useRoute } from '@react-navigation/native';
import AppLayout from "../../layouts/AppLayout"
import eventStyles from "../../styles/eventStyles"
import config from "../../config";
import Colors from "../../enums/colors";
import { getTextColorForBackground } from "../../utils/colorUtils";

function Header({ event }: { event: Event }) {
  const navigation = useNavigation<any>();

  return (
    <View style={eventStyles.eventPageHeader}>
      <Pressable onPress={() => navigation.navigate('Event', { event })} style={eventStyles.backButton}>
        <Text style={eventStyles.backButton}>&larr;</Text>
      </Pressable>
    </View>
  )
}

function QRCodeCard({ event, qrCode }: { event: Event, qrCode: string }) {
  const textColor = getTextColorForBackground(event.mainColorHex || Colors.DEFAULT_BUTTON_COLOR);
  
  return (
    <View style={[eventStyles.qrCodeCard, { backgroundColor: event.mainColorHex || Colors.DEFAULT_BUTTON_COLOR }]}>
      <View style={eventStyles.qrCodeBox}>
        {qrCode ? (
          <Image 
            source={{ uri: qrCode }} 
            style={eventStyles.qrCodeImage} 
          />
        ) : (
          <Text style={eventStyles.qrCodeLoadingText}>Generating QR Code...</Text>
        )}
      </View>
      
      <View style={eventStyles.qrCodeInfoSection}>
        <Text style={[eventStyles.qrCodeEventTitle, { color: textColor }]}>
          {event.title}
        </Text>
        <Text style={[eventStyles.qrCodeEventDate, { color: textColor }]}>
          {event.startDate.toLocaleDateString()}
        </Text>
      </View>
    </View>
  )
}

export default function QRCodePage() {
  const route = useRoute<any>();
  const { event }: { event: Event } = route.params;
  const [qrCode, setQrCode] = useState<string>('');
    
  const imgSrc = event.logoPath ? `${config.apiBaseUrl}/event${event.logoPath}` : null;

  useEffect(() => {
    async function GenerateQRCode() {
      try {
        const eventUrl = `${config.apiBaseUrl}/event/${event.idEvent}`;
        const qr = await QRCode.toDataURL(
          eventUrl, 
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
  }, [event.idEvent]);
    
  return (
    <AppLayout>
      <View style={eventStyles.eventPageContainer}>
        <Header event={event} /> 

        <View style={eventStyles.qrCodePageWrapper}>
          <Text style={eventStyles.qrCodeTitle}>Attendance QR Code</Text>
          
          <QRCodeCard event={event} qrCode={qrCode} />

          <Text style={eventStyles.qrCodeInstructions}>
            Scan this QR code to mark your attendance
          </Text>
        </View>
      </View>
    </AppLayout>
  )
}