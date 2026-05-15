import { Text, View } from "react-native";
import AppLayout from "../../layouts/AppLayout";
import attendanceStyle from "../../styles/attendanceStyle";
import { useState } from "react";

// TODO Mobile QR code scanner

export default function EventsAttendance() {
  const [qrResult, setQrResult] = useState<string>('');
  
  const handleScan = (result: any) => {
    if(!result) return;
    console.log(result);
    
    setQrResult(result?.text || '');
  }

  return (
    <AppLayout>
      <View style={attendanceStyle.attendanceContainer}>
        <Text>Scan a QR Code</Text>

        <Text>Mobile TODO</Text>
        
        {qrResult ? (
          <Text>Scanned QR Code: {qrResult}</Text>
        ) : (
          <Text>Waiting for scan...</Text>
        )}
      </View>
    </AppLayout>
  );
}