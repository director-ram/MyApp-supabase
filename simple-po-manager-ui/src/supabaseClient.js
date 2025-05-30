import { createClient } from '@supabase/supabase-js';

const supabaseUrl = 'https://eyfzxktwarebubzcfdhk.supabase.co'; // Your Supabase project URL
const supabaseKey = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImV5Znp4a3R3YXJlYnViemNmZGhrIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDYzMDAyMzIsImV4cCI6MjA2MTg3NjIzMn0._CqI06xFDYRmknP__cNlyoIPogLFyvK_saQEtsm8GHU';

export const supabase = createClient(supabaseUrl, supabaseKey); 