ALTER TABLE telegram_chat
ADD COLUMN current_scenario INT NULL;

ALTER TABLE telegram_chat
ADD COLUMN current_state INT NULL;

ALTER TABLE telegram_chat
ADD COLUMN current_scenario_args JSONB NULL;