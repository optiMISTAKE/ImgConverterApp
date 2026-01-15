import { Link } from "react-router-dom";
import { Container, Title, Text, Button, Stack, Center } from "@mantine/core";
import { IconHome } from "@tabler/icons-react";

export default function NotFound() {
    return (
        <Container size="md">
            <Center style={{ height: '80vh' }}>
                <Stack align="center" gap="md">
                    {/* Large "404" background text or style */}
                    <Title 
                        style={{ fontSize: '100px', fontWeight: 900, opacity: 0.1 }}
                    >
                        404
                    </Title>
                    
                    <Title order={1} ta="center">
                        Page Not Found
                    </Title>
                    
                    <Text c="dimmed" size="lg" ta="center" style={{ maxWidth: 500 }}>
                        The page you are looking for might have been moved, deleted, 
                        or does not exist.
                    </Text>

                    <Button 
                        component={Link} 
                        to="/" 
                        size="lg" 
                        variant="light"
                        leftSection={<IconHome size={20} />}
                    >
                        Return to main page
                    </Button>
                </Stack>
            </Center>
        </Container>
    );
}