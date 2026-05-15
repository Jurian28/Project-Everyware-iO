import { useEffect, useState } from "react";
import QRCode from "qrcode";
import { Pressable, Text, View, Image } from "react-native"
import { Event } from '../../services/EventService';
import { useNavigation, useRoute } from '@react-navigation/native';
import AppLayout from "../../layouts/AppLayout"
import eventStyles from "../../styles/eventStyles"
import config from "../../config";
import Colors from "../enums/colors";

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

export default function QRCodePage() {
  const route = useRoute<any>();
  const { event }: { event: Event } = route.params;
  const [qrCode, setQrCode] = useState<string>('');
    
  const imgSrc = event.logoPath ? `${config.apiBaseUrl}/event${event.logoPath}` : null;

  useEffect(() => {
    async function GenerateQRCode() {
      try {
        const qr = await QRCode.toDataURL(
          `Event: ${event.idEvent}`, 
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
              <Text style={eventStyles.qrCodeEventTitle}>
                {event.title}
              </Text>
              <Text style={eventStyles.qrCodeEventDate}>
                {event.startDate.toLocaleDateString()}
              </Text>
            </View>
          </View>

          <Text style={eventStyles.qrCodeInstructions}>
            Scan this QR code to mark your attendance
          </Text>
        </View>
      </View>
    </AppLayout>
  )
}